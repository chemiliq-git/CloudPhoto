namespace CloudPhoto.Services.Data.ImagiesService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Services.Data.TempCloudImageService;
    using CloudPhoto.Services.Mapping;
    using Microsoft.Extensions.Logging;

    public class ImagesService : IImagesService
    {
        public ImagesService(
            ILogger<ImagesService> logger,
            IDeletableEntityRepository<Image> imageRepository,
            IRepository<Vote> voteRepository,
            IRepository<UserSubscribe> userSubscribeRepository,
            IRepository<ImageTag> imageTagRepository,
            IRepository<Tag> tagRepository,
            IRepository<ImageCategory> imageCategoryRepository,
            IRepository<ApplicationUser> userRepository,
            ICategoriesService categoriesService,
            ITagsService tagsService,
            ITempCloudImagesService tempCloudImage)
        {
            this.Logger = logger;
            this.ImageRepository = imageRepository;
            this.VoteRepository = voteRepository;
            this.UserSubscribeRepository = userSubscribeRepository;
            this.ImageTagRepository = imageTagRepository;
            this.TagRepository = tagRepository;
            this.ImageCategoryRepository = imageCategoryRepository;
            this.CategoriesService = categoriesService;
            this.UserRepository = userRepository;
            this.TagsService = tagsService;
            this.TempCloudImage = tempCloudImage;
        }

        public ILogger<ImagesService> Logger { get; }

        public IDeletableEntityRepository<Image> ImageRepository { get; }

        public IRepository<Vote> VoteRepository { get; }

        public IRepository<UserSubscribe> UserSubscribeRepository { get; }

        public IRepository<ImageTag> ImageTagRepository { get; }

        public IRepository<Tag> TagRepository { get; }

        public IRepository<ImageCategory> ImageCategoryRepository { get; }

        public ICategoriesService CategoriesService { get; }

        public IRepository<ApplicationUser> UserRepository { get; }

        public ITagsService TagsService { get; }

        public ITempCloudImagesService TempCloudImage { get; }

        public async Task<string> CreateAsync(CreateImageModelData createData)
        {
            Category category = this.CategoriesService.GetByCategoryId<Category>(createData.CategoryId);

            if (category == null)
            {
                this.Logger.LogError($"Not exist category with Id:{createData.CategoryId}");
                return null;
            }

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

        public IEnumerable<T> GetMostLikeImageByCategory<T>(string categoryId, int countTopImage)
        {
            try
            {
                var selectTopVoteImage = (from image in this.ImageRepository.All()
                                          where image.ImageCategories.Where(x => x.CategoryId == categoryId).Any()
                                          let sumLikes = (from vote in this.VoteRepository.All() where vote.ImageId == image.Id select vote.IsLike).Sum()
                                          select new ImageLikeData
                                          {
                                              Image = image,
                                              LikeCounts = sumLikes,
                                          })
                         .OrderByDescending(x => x.LikeCounts);

                return selectTopVoteImage.Select(s => s.Image).Take(countTopImage).To<T>().ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(e, $"Error get top vote image from category: {categoryId}");
                return null;
            }
        }

        public IEnumerable<T> GetByFilter<T>(
            SearchImageData searchData,
            int perPage,
            int page = 1)
        {
            // add head select
            var selecTempData = from image in this.ImageRepository.All()
                                join user in this.UserRepository.All()
                                on image.AuthorId equals user.Id
                                let isFollow = (from subsribe in this.UserSubscribeRepository.All()
                                                where subsribe.UserSubscribedId == searchData.LikeForUserId
                                                && subsribe.SubscribeToUserId == user.Id
                                                select subsribe).Count()
                                let isLike = (from vote in this.VoteRepository.All()
                                              where vote.ImageId == image.Id
                                              && vote.AuthorId == searchData.LikeForUserId
                                              && vote.IsLike == 1
                                              select vote.IsLike).Sum()
                                let likeCount = (from vote in this.VoteRepository.All()
                                                 where vote.ImageId == image.Id
                                                 select vote.IsLike).Sum()
                                select new
                                {
                                    Image = image,
                                    User = user,
                                    IsFollow = isFollow >= 1,
                                    IsLike = isLike >= 1,
                                    LikeCount = likeCount,
                                };

            // filter by categories
            if (searchData.FilterCategory != null
              && searchData.FilterCategory.Count > 0)
            {
                selecTempData = from tempData in selecTempData
                                join imgCategory in this.ImageCategoryRepository.All()
                                on tempData.Image.Id equals imgCategory.ImageId
                                where searchData.FilterCategory.Contains(imgCategory.CategoryId)
                                select tempData;
            }

            // filter by tag
            if (searchData.FilterTags != null
                && searchData.FilterTags.Count > 0)
            {
                selecTempData = from tempData in selecTempData
                                join imageTag in this.ImageTagRepository.All()
                                on tempData.Image.Id equals imageTag.ImageId
                                where searchData.FilterTags.Contains(imageTag.TagId)
                                select tempData;
            }

            // filter images which like by user
            if (!string.IsNullOrEmpty(searchData.LikeByUser))
            {
                selecTempData = from tempData in selecTempData
                                join vote in this.VoteRepository.All()
                                on tempData.Image.Id equals vote.ImageId
                                where vote.AuthorId == searchData.LikeByUser
                                && vote.IsLike == 1
                                select tempData;
            }

            // get images upload by user
            if (!string.IsNullOrEmpty(searchData.AuthorId))
            {
                selecTempData = selecTempData.Where(tempData => tempData.Image.AuthorId == searchData.AuthorId);
            }

            // filter images by text tag
            if (!string.IsNullOrEmpty(searchData.FilterByTag))
            {
                selecTempData = selecTempData.Where(
                        tempData => (from imaTag in this.ImageTagRepository.All()
                                     join tag in this.TagRepository.All() on imaTag.TagId equals tag.Id
                                     where tag.Name.Contains(searchData.FilterByTag)
                                     select imaTag.ImageId).Contains(tempData.Image.Id));
            }

            selecTempData = selecTempData.OrderBy(tempData => tempData.Image.Id);
            selecTempData = selecTempData.Skip((page - 1) * perPage).Take(perPage);

            var selecResponseData = from tempData in selecTempData
                                    select new ResponseSearchImageModelData()
                                    {
                                        Id = tempData.Image.Id,
                                        Title = tempData.Image.Title,
                                        Description = tempData.Image.Description,
                                        ThumbnailImageUrl = tempData.Image.ThumbnailImageUrl,
                                        ImageUrl = tempData.Image.ImageUrl,
                                        ImageType = tempData.Image.ImageType,
                                        AuthorId = tempData.Image.AuthorId,
                                        AuthorAvatarUrl = tempData.User.UserAvatarUrl,
                                        PayPalEmail = tempData.User.PayPalEmail,
                                        IsFollow = tempData.IsFollow,
                                        IsLike = tempData.IsLike,
                                        LikeCount = tempData.LikeCount,
                                    };

            try
            {
                return selecResponseData.To<T>().ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(e, e.Message);
                return null;
            }
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
            if (tags == null)
            {
                return imageTags;
            }

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
