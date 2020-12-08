namespace CloudPhoto.Services.Data.ImagiesService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.DapperService;
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Services.ImageManipulationProvider;
    using CloudPhoto.Services.RemoteStorage;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class ImagesService : IImagesService
    {
        public ImagesService(
            ILogger<ImagesService> logger,
            IDeletableEntityRepository<Image> imageRepository,
            ICategoriesService categoriesService,
            ITagsService tagsService,
            IDapperService dapperService,
            IRemoteStorageService storageService,
            IImageManipulationProvider imageManipulation,
            IConfiguration configuration)
        {
            this.Logger = logger;
            this.ImageRepository = imageRepository;
            this.CategoriesService = categoriesService;
            this.TagsService = tagsService;
            this.DapperService = dapperService;
            this.ImageManipulation = imageManipulation;
            this.Configuration = configuration;
            this.StorageService = storageService;
        }

        public ILogger<ImagesService> Logger { get; }

        public IDeletableEntityRepository<Image> ImageRepository { get; }

        public ICategoriesService CategoriesService { get; }

        public ITagsService TagsService { get; }

        public IDapperService DapperService { get; }

        public IImageManipulationProvider ImageManipulation { get; }

        public IConfiguration Configuration { get; }

        public IRemoteStorageService StorageService { get; }

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

            image.ThumbnailImageUrl = await this.GenerateThumbnailImage(rootFolder, createData.ImageUrl);

            image.ImageTags = await this.ParseImageTag(image, createData.Tags);
            image.ImageCategories = new List<ImageCategory>() { new ImageCategory() { CategoryId = category.Id, ImageId = image.Id } };

            await this.ImageRepository.AddAsync(image);

            await this.ImageRepository.SaveChangesAsync();

            await this.UploadOriginalFile(createData.Id, rootFolder, createData.ImageUrl);

            return image.Id;
        }

        public Task<bool> Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public T GetByCategoryId<T>(string categoryId)
        {
            throw new System.NotImplementedException();
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
                LikeForUserId = searchData.LikeForUserId,
                @AuthorId = searchData.AuthorId,
                @FilterCategory = searchData.FilterCategory.ToArray(),
                @FilterTag = searchData.FilterByTag,
                @FilterTags = searchData.FilterTags,
                @LikeByUser = searchData.LikeByUser,
            };

            StringBuilder sqlSelect = new StringBuilder();

            // add head select
            sqlSelect.Append(
                @"SELECT 
                    i.*,
                    c.ClaimValue AS UserAvatar,
				    (CASE
                    WHEN v.IsLike IS NULL THEN 0
                    WHEN v.IsLike = 1 THEN 1
	                ELSE 0
                    END) AS IsLike,
                    (SELECT SUM(IsLike) FROM Votes Where Votes.ImageId = i.Id) AS LikeCount
                    FROM Images AS i
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
            var d = this.DapperService.GetAll<T>(sqlSelect.ToString(), parameters, commandType: CommandType.Text);
            return d;
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

        private async Task UploadOriginalFile(string id, string rootFolder, string imageUrl)
        {
            try
            {
                string fullPath = rootFolder + imageUrl;
                using (FileStream str = new FileStream(fullPath, FileMode.Open))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        str.CopyTo(memory);
                        memory.Position = 0;
                        StoreFileInfo storeFile = await this.StorageService.UploadFile(
                            new UploadDataInfo(
                                Path.GetFileName(fullPath),
                                memory,
                                "WebPictures",
                                string.Empty));

                        Image image = this.ImageRepository.All().First(image => image.Id == id);
                        image.ImageUrl = storeFile.FileAddress;
                        this.ImageRepository.Update(image);
                        await this.ImageRepository.SaveChangesAsync();
                    }
                }

                this.DeleteLocalFiles(fullPath);
            }
            catch (Exception e)
            {
                this.Logger.LogError("Error upload original file", e);
            }
        }

        private void DeleteLocalFiles(string fullPath)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(fullPath);
                Directory.Delete(directoryName, true);
            }
            catch (Exception e)
            {
                this.Logger.LogError("Error delete local files", e);
            }
        }

        private async Task<string> GenerateThumbnailImage(string rootFolder, string imageUrl)
        {
            string fullPath = rootFolder + imageUrl;
            using FileStream str = new FileStream(fullPath, FileMode.Open);
            using MemoryStream memory = new MemoryStream();
            str.CopyTo(memory);
            memory.Position = 0;
            byte[] image = this.ImageManipulation.Resize(
                memory,
                int.Parse(this.Configuration.GetSection("Images:ThumbnailImageWidth").Value),
                int.Parse(this.Configuration.GetSection("Images:ThumbnailImageHeight").Value));

            using MemoryStream cropImage = new MemoryStream(image);
            StoreFileInfo storeFile = await this.StorageService.UploadFile(
                new UploadDataInfo(
                    Path.GetFileName(fullPath),
                    cropImage,
                    "WebPictures",
                    string.Empty));
            return storeFile.FileAddress;
        }
    }
}
