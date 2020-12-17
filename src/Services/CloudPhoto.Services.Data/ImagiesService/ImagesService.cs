namespace CloudPhoto.Services.Data.ImagiesService
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.DapperService;
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Services.Data.TempCloudImageService;
    using CloudPhoto.Services.Mapping;
    using Microsoft.Extensions.Logging;

    public class ImagesService : IImagesService
    {
        public ImagesService(
            ILogger<ImagesService> logger,
            IDeletableEntityRepository<Image> imageRepository,
            ICategoriesService categoriesService,
            ITagsService tagsService,
            IDapperService dapperService,
            ITempCloudImageService tempCloudImage)
        {
            this.Logger = logger;
            this.ImageRepository = imageRepository;
            this.CategoriesService = categoriesService;
            this.TagsService = tagsService;
            this.DapperService = dapperService;
            this.TempCloudImage = tempCloudImage;
        }

        public ILogger<ImagesService> Logger { get; }

        public IDeletableEntityRepository<Image> ImageRepository { get; }

        public ICategoriesService CategoriesService { get; }

        public ITagsService TagsService { get; }

        public IDapperService DapperService { get; }

        public ITempCloudImageService TempCloudImage { get; }

        public async Task<string> CreateAsync(string rootFolder, CreateImageModelData createData)
        {
            Category category = this.CategoriesService.GetByCategoryId<Category>(createData.CategoryId);

            var image = new Image
            {
                Id = createData.Id,
                Title = createData.Title,
                Description = createData.Description,
                AuthorId = createData.AuthorId,
            };

            List<TempCloudImage> lstUploadImages = this.TempCloudImage.GetByImageId<TempCloudImage>(image.Id).ToList();

            image.ThumbnailImageUrl = lstUploadImages.Find(temp => temp.ImageType == (int)ImageType.Thumbnail)?.ImageUrl;
            if (string.IsNullOrEmpty(image.ThumbnailImageUrl))
            {
                return null;
            }

            image.ImageUrl = lstUploadImages.Find(temp => temp.ImageType == (int)ImageType.Original)?.ImageUrl;
            if (string.IsNullOrEmpty(image.ImageUrl))
            {
                return null;
            }

            image.ImageTags = await this.ParseImageTag(image, createData.Tags);
            image.ImageCategories = new List<ImageCategory>() { new ImageCategory() { CategoryId = category.Id, ImageId = image.Id } };

            await this.ImageRepository.AddAsync(image);

            await this.ImageRepository.SaveChangesAsync();

            return image.Id;
        }

        public Task<bool> Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetMostLikeImageByCategory<T>(string categoryId, int countTopImage)
        {
            StringBuilder sqlSelect = new StringBuilder(
                @"SELECT TOP(@countTopImage) *,
                (SELECT SUM(v.IsLike) from Votes AS v
                WHERE i.Id = v.ImageId) AS ImageLikes
                FROM Images as i
                JOIN ImageCategories AS ic ON ic.CategoryId =@categoryId ANd ic.ImageId = i.Id
                WHERE ic.CategoryId = @categoryId
                ORDER BY ImageLikes Desc");
            var parameters = new
            {
                categoryId,
                countTopImage,
            };

            return this.DapperService.GetAll<T>(sqlSelect.ToString(), parameters, commandType: CommandType.Text);
        }

        public IEnumerable<T> GetByFilter<T>(
            SearchImageData searchData,
            int perPage,
            int page = 1)
        {
            var parameters = new
            {
                Skip = (page - 1) * perPage,
                Take = perPage,
                ClaimType = GlobalConstants.ExternalClaimAvatar,
                searchData.LikeForUserId,
                searchData.AuthorId,
                @FilterCategory = searchData.FilterCategory.ToArray(),
                @FilterTag = searchData.FilterByTag,
                searchData.FilterTags,
                searchData.LikeByUser,
            };

            StringBuilder sqlSelect = new StringBuilder();

            // add head select
            sqlSelect.Append(
                @"SELECT 
                    i.*,
                    aspu.FirstName + ' ' + aspu.LastName AS AuthorFullName,
                    aspu.Email As AuthorEmail, 
                    aspu.PayPalEmail,
                    c.ClaimValue AS AuthorAvatarUrl,
                    -- get follow info    
                    (SELECT Count(*) FROM UserSubscribes AS us
					WHERE us.UserSubscribedId = @LikeForUserId
					AND aspu.Id = us.SubscribeToUserId) AS IsFollow,
				    -- get like counts
                    (CASE
                    WHEN v.IsLike IS NULL THEN 0
                    WHEN v.IsLike = 1 THEN 1
	                ELSE 0
                    END) AS IsLike,
                    (SELECT SUM(IsLike) FROM Votes Where Votes.ImageId = i.Id) AS LikeCount
                    FROM Images AS i
                    JOIN AspNetUsers AS aspu ON aspu.Id = i.AuthorId 
                    LEFT JOIN AspNetUserClaims AS c On i.AuthorId = c.UserId AND c.ClaimType = @ClaimType
                    LEFT JOIN Votes AS v ON v.AuthorId = @LikeForUserId AND v.ImageId = i.Id AND v.IsLike = 1");

            // filter by categories
            if (searchData.FilterCategory != null
              && searchData.FilterCategory.Count > 0)
            {
                sqlSelect.AppendLine("JOIN ImageCategories AS ic ON ic.ImageId = i.Id AND ic.CategoryId in @FilterCategory");
            }

            // filter by tag
            if (searchData.FilterTags != null
                && searchData.FilterTags.Count > 0)
            {
                sqlSelect.AppendLine("JOIN ImageTags AS it ON it.ImageId = i.Id AND it.TagId in @FilterTags");
            }

            // filter images which like by user
            if (!string.IsNullOrEmpty(searchData.LikeByUser))
            {
                sqlSelect.AppendLine("JOIN Votes AS vt ON vt.ImageId = i.Id AND vt.AuthorId = @LikeByUser AND vt.IsLike = 1");
            }

            sqlSelect.AppendLine("WHERE (1=1)");

            // get images upload by user
            if (!string.IsNullOrEmpty(searchData.AuthorId))
            {
                sqlSelect.AppendLine("AND i.AuthorId = @AuthorId");
            }

            // filter images by text tag
            if (!string.IsNullOrEmpty(searchData.FilterByTag))
            {
                sqlSelect.AppendLine(@"AND i.Id in(SELECT ImageId FROM ImageTags
                          JOIN Tags ON Tags.Id = ImageTags.TagId
                          WHERE Tags.Name like '%' + @FilterTag + '%')");
            }

            sqlSelect.AppendLine("ORDER BY i.ID OFFSET @Skip ROWS ");
            sqlSelect.AppendLine("FETCH NEXT @Take ROWS ONLY");
            return this.DapperService.GetAll<T>(sqlSelect.ToString(), parameters, commandType: CommandType.Text);
        }

        public T GetImageById<T>(string imageId)
        {
            IQueryable<Image> query =
                this.ImageRepository.All()
                .Where(c => c.Id == imageId);

            return query.To<T>().FirstOrDefault();
        }

        private async Task<ICollection<ImageTag>> ParseImageTag(Image image, List<string> tags)
        {
            List<ImageTag> imageTags = new List<ImageTag>();
            Tag tempTag;

            foreach (string tag in tags)
            {
                tempTag = this.TagsService.GetByTagName<Tag>(tag);
                if (tempTag != null)
                {
                    imageTags.Add(new ImageTag() { ImageId = image.Id, TagId = tempTag.Id });
                }
                else
                {
                    tempTag = new Tag()
                    {
                        Name = tag,
                        Description = tag,
                        AuthorId = image.AuthorId,
                    };

                    tempTag.Id = await this.TagsService.CreateAsync(tag, tag, image.AuthorId);
                    imageTags.Add(new ImageTag() { ImageId = image.Id, TagId = tempTag.Id });
                }
            }

            return imageTags;
        }
    }
}
