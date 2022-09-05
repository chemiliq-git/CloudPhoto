namespace CloudPhoto.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using CategoriesService;
    using ImagiesService;
    using TagsService;
    using TempCloudImageService;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class CategoriesServiceTest : IDisposable
    {
        private const string TestCategory1 = "Category1";
        private const string TestCategory2 = "Category2";
        private const string TestCategory3 = "Category3";

        private const string TestImage1 = "testImage1";
        private const string TestImage2 = "testImage2";
        private const string TestImage3 = "testImage3";

        private const string TestUserId = "testUserId";

        private string testCategoryId1;
        private string testCategoryId2;
        private string testCategoryId3;

        private IDeletableEntityRepository<Category> categoriesRepository;
        private EfRepository<Vote> voteRepository;
        private EfDeletableEntityRepository<Image> imageRepository;
        private EfDeletableEntityRepository<ImageCategory> imageCategoryRepository;
        private CategoriesService categoriesService;
        private ImagesService imagesService;
        private TempCloudImagesService tempCloudImagesService;

        public CategoriesServiceTest()
        {
            InitTestServices();

            AddTestData();
        }

        [Fact]
        public void GetAllCategories()
        {
            IEnumerable<Category> lstCategories = categoriesService.GetAll<Category>();
            Assert.Equal(3, lstCategories?.Count());
        }

        [Fact]
        public async void CreateCategory()
        {
            string newId = await categoriesService.CreateAsync(null, "name", "undefine");
            Assert.Null(newId);
        }

        [Fact]
        public void GetMostLikedHaveOnlyThirdCategory()
        {
            IEnumerable<Category> lstCategories = categoriesService.GetMostLikedCategory<Category>(5);
            Assert.Equal(3, lstCategories?.Count());
        }

        [Fact]
        public void GetMostLikedTestTopCount()
        {
            IEnumerable<Category> lstCategories = categoriesService.GetMostLikedCategory<Category>(2);
            Assert.Equal(2, lstCategories?.Count());
        }

        [Fact]
        public async void UpdateCategory()
        {
            bool isUpdate = await categoriesService.UpdateAsync(testCategoryId1, "UpdateName", "UpdateDescription");
            if (!isUpdate)
            {
                Assert.True(isUpdate);
            }

            Category updateCategory = categoriesService.GetByCategoryId<Category>(testCategoryId1);
            Assert.Equal("UpdateName", updateCategory?.Name);
        }

        [Fact]
        public async void UpdateNotExistCategoryShouldFail()
        {
            bool isUpdate = await categoriesService.UpdateAsync("undefine", "UpdateName", "UpdateDescription");
            Assert.False(isUpdate);
        }

        [Fact]
        public async void UpdateCategoryWrongParamsShouldFail()
        {
            bool isUpdate = await categoriesService.UpdateAsync(null, null, "UpdateDescription");
            Assert.False(isUpdate);
        }

        [Fact]
        public async void UpdateCategoryEmptyNameShouldFail()
        {
            bool isUpdate = await categoriesService.UpdateAsync(testCategoryId1, null, "UpdateDescription");
            Assert.False(isUpdate);
        }

        [Fact]
        public async void DeleteCategory()
        {
            bool isDelete = await categoriesService.Delete(testCategoryId3);
            Assert.True(isDelete);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("undefine")]
        public async void DeleteCategoryShouldFalse(string categoryId)
        {
            bool isDelete = await categoriesService.Delete(categoryId);
            Assert.False(isDelete);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                categoriesRepository.Dispose();
                voteRepository.Dispose();
                imageRepository.Dispose();
                imageCategoryRepository.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            ApplicationDbContext dbContext = new ApplicationDbContext(options.Options);

            imageRepository = new EfDeletableEntityRepository<Image>(dbContext);

            imageCategoryRepository = new EfDeletableEntityRepository<ImageCategory>(dbContext);

            voteRepository = new EfRepository<Vote>(dbContext);

            categoriesRepository = new EfDeletableEntityRepository<Category>(dbContext);
            var loggerCategory = Mock.Of<ILogger<CategoriesService>>();

            categoriesService = new CategoriesService(
                loggerCategory,
                categoriesRepository,
                voteRepository,
                imageRepository,
                imageCategoryRepository);

            CreateImageService(dbContext);
        }

        private void CreateImageService(ApplicationDbContext dbContext)
        {
            var tagRepository = new EfDeletableEntityRepository<Tag>(dbContext);
            var tagService = new TagsService(tagRepository);

            var userSubscribeRepository = new EfDeletableEntityRepository<UserSubscribe>(dbContext);
            var imageTagRepository = new EfDeletableEntityRepository<ImageTag>(dbContext);
            var imageCategoryRepository = new EfDeletableEntityRepository<ImageCategory>(dbContext);
            var userRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var tempCloudImageRepository = new EfRepository<TempCloudImage>(dbContext);
            tempCloudImagesService = new TempCloudImagesService(tempCloudImageRepository);

            var logger = Mock.Of<ILogger<ImagesService>>();

            imagesService = new ImagesService(
                logger,
                imageRepository,
                voteRepository,
                userSubscribeRepository,
                imageTagRepository,
                tagRepository,
                imageCategoryRepository,
                userRepository,
                categoriesService,
                tagService,
                tempCloudImagesService);
        }

        private async void AddTestData()
        {
            testCategoryId1 = await categoriesService.CreateAsync(TestCategory1, TestCategory1, "undefine");
            testCategoryId2 = await categoriesService.CreateAsync(TestCategory2, TestCategory2, "undefine");
            testCategoryId3 = await categoriesService.CreateAsync(TestCategory3, TestCategory3, "undefine");

            await CreateImageTempData(TestImage1);
            await CreateImageTempData(TestImage2);
            await CreateImageTempData(TestImage3);

            await imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = TestImage1,
                AuthorId = TestUserId,
                CategoryId = testCategoryId1,
            });

            await imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = TestImage2,
                AuthorId = TestUserId,
                CategoryId = testCategoryId2,
            });

            await imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = TestImage3,
                AuthorId = TestUserId,
                CategoryId = testCategoryId2,
            });

            await voteRepository.AddAsync(new Vote() { ImageId = TestImage1, IsLike = 1, AuthorId = TestUserId });
            await voteRepository.AddAsync(new Vote() { ImageId = TestImage1, IsLike = 1, AuthorId = TestUserId });
            await voteRepository.AddAsync(new Vote() { ImageId = TestImage2, IsLike = 1, AuthorId = TestUserId });
        }

        private async Task CreateImageTempData(string imageId)
        {
            await tempCloudImagesService.CreateAsync(new TempCloudImage()
            {
                ImageId = imageId,
                ImageUrl = "tempURL",
                ImageType = (int)ImageType.Original,
            });

            await tempCloudImagesService.CreateAsync(new TempCloudImage()
            {
                ImageId = imageId,
                ImageUrl = "tempURL",
                ImageType = (int)ImageType.Thumbnail,
            });
        }
    }
}
