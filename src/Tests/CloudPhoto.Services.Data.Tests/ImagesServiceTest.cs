namespace CloudPhoto.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Services.Data.TempCloudImageService;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class ImagesServiceTest : IDisposable
    {
        private const string TestCategory1 = "Category1";

        private const string TestTag1 = "Tag1";
        private const string TestTag2 = "Tag2";

        private const string TestImage1 = "testImage1";
        private const string TestImage2 = "testImage2";

        private const string TestUserId1 = "TestUser1";
        private const string TestUserId2 = "TestUser2";

        private EfDeletableEntityRepository<Image> imageRepository;
        private EfDeletableEntityRepository<ApplicationUser> userRepository;
        private EfDeletableEntityRepository<ImageCategory> imageCategoryRepository;
        private EfDeletableEntityRepository<ImageTag> imageTagRepository;
        private EfRepository<Vote> voteRepository;
        private CategoriesService categoriesService;
        private TagsService tagService;
        private TempCloudImagesService tempCloudImageService;
        private ImagesService imagesService;

        private string testCategoryId1;
        private string testTagId2;

        public ImagesServiceTest()
        {
            this.InitTestServices();

            this.AddTestData();
        }

        [Fact]
        public async void AddImageFailBecauseMissingCategory()
        {
            string newImageID = await this.imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = "memoryImageId",
                AuthorId = "memoryAuthorId",
            });

            Assert.Null(newImageID);
        }

        [Fact]
        public async void AddImageFailBecauseMissingOriginalImage()
        {
            string imageId = "missingOriginalImage";
            string categoryId = await this.categoriesService.CreateAsync("TestCategory", null, "memoryUserId");

            await this.tempCloudImageService.CreateAsync(new TempCloudImage()
            {
                ImageId = imageId,
                ImageUrl = "tempURL",
                ImageType = (int)ImageType.Thumbnail,
            });

            string newImageID = await this.imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = imageId,
                AuthorId = "memoryAuthorId",
                CategoryId = categoryId,
            });

            Assert.Null(newImageID);
        }

        [Fact]
        public async void AddImageFailBecauseMissingThumbnailImage()
        {
            string imageId = "missingThumbnailImage";
            string categoryId = await this.categoriesService.CreateAsync("TestCategory", null, "memoryUserId");

            await this.tempCloudImageService.CreateAsync(new TempCloudImage()
            {
                ImageId = "memoryImageId",
                ImageUrl = "tempURL",
                ImageType = (int)ImageType.Thumbnail,
            });

            string newImageID = await this.imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = imageId,
                AuthorId = "memoryAuthorId",
                CategoryId = categoryId,
            });

            Assert.Null(newImageID);
        }

        [Fact]
        public async void AddImageAsync()
        {
            string imageId = "succesAddImage";
            string categoryId = await this.categoriesService.CreateAsync("TestCategory", null, "memoryUserId");

            await this.tempCloudImageService.CreateAsync(new TempCloudImage()
            {
                ImageId = imageId,
                ImageUrl = "tempURL",
                ImageType = (int)ImageType.Original,
            });

            await this.tempCloudImageService.CreateAsync(new TempCloudImage()
            {
                ImageId = imageId,
                ImageUrl = "tempURL",
                ImageType = (int)ImageType.Thumbnail,
            });

            string newImageID = await this.imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = imageId,
                AuthorId = "memoryAuthorId",
                CategoryId = categoryId,
            });

            Assert.Equal(imageId, newImageID);
        }

        [Fact]
        public void GetImageByIdCouldFailBecouseMissingImageId()
        {
            Image selectImage = this.imagesService.GetImageById<Image>("missingImageId");

            Assert.Null(selectImage);
        }

        [Fact]
        public void GetImageById()
        {
            Image selectImage = this.imagesService.GetImageById<Image>(TestImage1);
            Assert.NotNull(selectImage);
        }

        [Fact]
        public void GetMostLikeImageFirstCase()
        {
            List<Image> lstResult = this.imagesService.GetMostLikeImageByCategory<Image>(this.testCategoryId1, 4).ToList();
            Assert.Equal(2, lstResult.Count);
        }

        [Fact]
        public void GetMostLikeImageSecondCase()
        {
            List<Image> lstResult = this.imagesService.GetMostLikeImageByCategory<Image>(this.testCategoryId1, 1).ToList();
            Assert.Single(lstResult);
        }

        [Fact]
        public void GetMostLikeImageThirdCase()
        {
            List<Image> lstResult = this.imagesService.GetMostLikeImageByCategory<Image>(this.testCategoryId1, 2).ToList();
            Assert.Equal(lstResult?[0].Id, TestImage1);
        }

        [Fact]
        public void GetImageByCategory()
        {
            SearchImageData searchData = new SearchImageData
            {
                FilterCategory = new List<string>() { this.testCategoryId1 },
            };
            var selectImages = this.imagesService.GetByFilter<ResponseSearchImageModelData>(searchData, 10, 1);
            Assert.Equal(2, selectImages.Count());
        }

        [Fact]
        public void GetImageByTagShouldReturnOneImage()
        {
            SearchImageData searchData = new SearchImageData
            {
                FilterByTag = TestTag2,
            };
            var selectImages = this.imagesService.GetByFilter<ResponseSearchImageModelData>(searchData, 10, 1);
            Assert.Single(selectImages);
        }

        [Fact]
        public void GetImageByTagShoulReturnTwoImage()
        {
            SearchImageData searchData = new SearchImageData
            {
                FilterByTag = TestTag1,
            };
            var selectImages = this.imagesService.GetByFilter<ResponseSearchImageModelData>(searchData, 10, 1);
            Assert.Equal(2, selectImages.Count());
        }

        [Fact]
        public void GetImageByTags()
        {
            SearchImageData searchData = new SearchImageData
            {
                FilterTags = new List<string>() { this.testTagId2 },
            };
            var selectImages = this.imagesService.GetByFilter<ResponseSearchImageModelData>(searchData, 10, 1);
            Assert.Single(selectImages);
        }

        [Fact]
        public void GetImageByAuthor()
        {
            SearchImageData searchData = new SearchImageData
            {
                AuthorId = TestUserId1,
            };
            var selectImages = this.imagesService.GetByFilter<ResponseSearchImageModelData>(searchData, 10, 1);
            Assert.Equal(2, selectImages.Count());
        }

        [Fact]
        public void GetImageTestPaging()
        {
            SearchImageData searchData = new SearchImageData();
            var selectImages = this.imagesService.GetByFilter<ResponseSearchImageModelData>(searchData, 1, 1);
            Assert.Single(selectImages);
        }

        [Fact]
        public void GetImageBySearchData()
        {
            SearchImageData searchData = new SearchImageData()
            {
                LikeByUser = TestUserId2,
                LikeForUserId = TestUserId1,
            };

            var selectImages = this.imagesService.GetByFilter<ResponseSearchImageModelData>(searchData, 10, 1);
            Assert.Single(selectImages);
        }

        [Fact]
        public void GetImageCheckIsLikeFlag()
        {
            SearchImageData searchData = new SearchImageData()
            {
                LikeByUser = TestUserId2,
                LikeForUserId = TestUserId1,
            };

            var selectImages = this.imagesService.GetByFilter<ResponseSearchImageModelData>(searchData, 10, 1);
            Assert.True(selectImages.ToList()[0].IsLike);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.imageRepository.Dispose();
                this.voteRepository.Dispose();
                this.userRepository.Dispose();
                this.imageCategoryRepository.Dispose();
                this.imageTagRepository.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            ApplicationDbContext dbContext = new ApplicationDbContext(options.Options);

            this.imageRepository = new EfDeletableEntityRepository<Image>(dbContext);

            var tagRepository = new EfDeletableEntityRepository<Tag>(dbContext);
            this.tagService = new TagsService(tagRepository);

            this.imageCategoryRepository = new EfDeletableEntityRepository<ImageCategory>(dbContext);

            var tempCloudImageRepository = new EfRepository<TempCloudImage>(dbContext);
            this.tempCloudImageService = new TempCloudImagesService(tempCloudImageRepository);

            this.voteRepository = new EfRepository<Vote>(dbContext);

            var userSubscribeRepository = new EfDeletableEntityRepository<UserSubscribe>(dbContext);
            this.imageTagRepository = new EfDeletableEntityRepository<ImageTag>(dbContext);
            this.userRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var categoryRepository = new EfDeletableEntityRepository<Category>(dbContext);
            var loggerCategory = Mock.Of<ILogger<CategoriesService>>();
            this.categoriesService = new CategoriesService(
                loggerCategory,
                categoryRepository,
                this.voteRepository,
                this.imageRepository,
                this.imageCategoryRepository);

            var logger = Mock.Of<ILogger<ImagesService>>();

            this.imagesService = new ImagesService(
                logger,
                this.imageRepository,
                this.voteRepository,
                userSubscribeRepository,
                this.imageTagRepository,
                tagRepository,
                this.imageCategoryRepository,
                this.userRepository,
                this.categoriesService,
                this.tagService,
                this.tempCloudImageService);
        }

        private async void AddTestData()
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                Id = TestUserId1,
                Email = "Test@email.com",
            };
            await this.userRepository.AddAsync(applicationUser);
            applicationUser = new ApplicationUser
            {
                Id = TestUserId2,
                Email = "Test@email.com",
            };
            await this.userRepository.AddAsync(applicationUser);

            this.testCategoryId1 = await this.categoriesService.CreateAsync(TestCategory1, TestCategory1, TestUserId1);

            await this.CreateImageTempData(TestImage1);
            await this.CreateImageTempData(TestImage2);

            await this.tagService.CreateAsync(TestTag1, null, TestUserId1);
            this.testTagId2 = await this.tagService.CreateAsync(TestTag2, null, TestUserId1);

            await this.imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = TestImage1,
                AuthorId = TestUserId1,
                CategoryId = this.testCategoryId1,
                Tags = new List<string>() { TestTag1 },
            });

            await this.imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = TestImage2,
                AuthorId = TestUserId1,
                CategoryId = this.testCategoryId1,
                Tags = new List<string>() { TestTag1, TestTag2 },
            });

            await this.voteRepository.AddAsync(new Vote() { ImageId = TestImage1, IsLike = 1, AuthorId = TestUserId1 });
            await this.voteRepository.AddAsync(new Vote() { ImageId = TestImage1, IsLike = 1, AuthorId = TestUserId2 });

            await this.voteRepository.AddAsync(new Vote() { ImageId = TestImage2, IsLike = 1, AuthorId = TestUserId1 });

            await this.voteRepository.SaveChangesAsync();
        }

        private async Task CreateImageTempData(string imageId)
        {
            await this.tempCloudImageService.CreateAsync(new TempCloudImage()
            {
                ImageId = imageId,
                ImageUrl = "tempURL",
                ImageType = (int)ImageType.Original,
            });

            await this.tempCloudImageService.CreateAsync(new TempCloudImage()
            {
                ImageId = imageId,
                ImageUrl = "tempURL",
                ImageType = (int)ImageType.Thumbnail,
            });
        }
    }
}
