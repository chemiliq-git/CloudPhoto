namespace CloudPhoto.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.DapperService;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Services.Data.TempCloudImageService;
    using CloudPhoto.Services.Data.Tests.Configure;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using ServiceStack.OrmLite;
    using Xunit;

    public class ImagesServiceTest : IDisposable
    {
        private const string TestCategory1 = "Category1";
        private const string TestCategory2 = "Category2";

        private const string TestTag1 = "Tag1";
        private const string TestTag2 = "Tag2";

        private const string TestImage1 = "testImage1";
        private const string TestImage2 = "testImage2";

        private const string TestUserId1 = "TestUser1";

        private EfDeletableEntityRepository<Image> imageRepository;
        private DapperService dapperService;
        private CategoriesService categoriesService;
        private TagService tagService;
        private TempCloudImageService tempCloudImageService;
        private ImagesService imagesService;

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
        public async void GetImageById()
        {
            //string imageId = "getAddImage";
            //string categoryId = await this.categoriesService.CreateAsync("TestCategory", null, "memoryUserId");

            //await this.tempCloudImageService.CreateAsync(new TempCloudImage()
            //{
            //    ImageId = imageId,
            //    ImageUrl = "tempURL",
            //    ImageType = (int)ImageType.Original,
            //});

            //await this.tempCloudImageService.CreateAsync(new TempCloudImage()
            //{
            //    ImageId = imageId,
            //    ImageUrl = "tempURL",
            //});

            //string newImageID = await this.imagesService.CreateAsync(new CreateImageModelData()
            //{
            //    Id = imageId,
            //    AuthorId = "memoryAuthorId",
            //    CategoryId = categoryId,
            //});

            Image selectImage = this.imagesService.GetImageById<Image>(TestImage1);
            Assert.NotNull(selectImage);
        }

        //[Fact]
        //public void GetByFilter()
        //{
        //    SearchImageData searchImageData = new SearchImageData()
        //    {
        //        FilterByTag = TestTag1,
        //    };

        //    List<Image> lstResult = this.imagesService.GetByFilter<Image>(searchImageData, 10, 1)?.ToList();
        //    Assert.Equal(2, lstResult.Count);
        //}

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
                this.dapperService.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            ApplicationDbContext dbContext = new ApplicationDbContext(options.Options);

            this.imageRepository = new EfDeletableEntityRepository<Image>(dbContext);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "DefaultConnection", "NoConnectionString" },
            }).Build();

            this.dapperService = new DapperService(configuration);

            var categoryRepository = new EfDeletableEntityRepository<Category>(dbContext);
            this.categoriesService = new CategoriesService(
                categoryRepository,
                this.dapperService);

            var tagRepository = new EfDeletableEntityRepository<Tag>(dbContext);
            this.tagService = new TagService(tagRepository);

            var tempCloudImageRepository = new EfRepository<TempCloudImage>(dbContext);
            this.tempCloudImageService = new TempCloudImageService(tempCloudImageRepository);

            var logger = Mock.Of<ILogger<ImagesService>>();

            this.imagesService = new ImagesService(
                logger,
                this.imageRepository,
                this.categoriesService,
                this.tagService,
                this.dapperService,
                this.tempCloudImageService);

        }

        private async void AddTestData()
        {
            string categoryId1 = await this.categoriesService.CreateAsync(TestCategory1, TestCategory1, TestUserId1);
            string categoryId2 = await this.categoriesService.CreateAsync(TestCategory2, null, TestUserId1);

            await this.CreateImageTempData(TestImage1);
            await this.CreateImageTempData(TestImage2);

            string tagId1 = await this.tagService.CreateAsync(TestTag1, null, TestUserId1);
            string tagId2 = await this.tagService.CreateAsync(TestTag2, null, TestUserId1);

            await this.imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = TestImage1,
                AuthorId = TestUserId1,
                CategoryId = categoryId1,
                Tags = new List<string>() { tagId1 },
            });

            await this.imagesService.CreateAsync(new CreateImageModelData()
            {
                Id = TestImage2,
                AuthorId = TestUserId1,
                CategoryId = categoryId2,
                Tags = new List<string>() { tagId1, tagId2 },
            });
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
