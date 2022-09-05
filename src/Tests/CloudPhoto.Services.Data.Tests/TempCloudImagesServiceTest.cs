namespace CloudPhoto.Services.Data.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using TempCloudImageService;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class TempCloudImagesServiceTest : IDisposable
    {
        private const string FirstTestImageId = "firstTestImage";
        private const string SecondTestImageId = "secondTestImage";
        private const string ThirdTestImageId = "thirdTestImage";

        private EfRepository<TempCloudImage> repository;
        private TempCloudImagesService service;

        public TempCloudImagesServiceTest()
        {
            InitTestServices();

            AddTestData();
        }

        [Fact]
        public void GetByImage()
        {
            IEnumerable<TempCloudImage> lstSelectImage = service.GetByImageId<TempCloudImage>(FirstTestImageId);
            Assert.Equal(2, lstSelectImage?.Count());
        }

        [Fact]
        public void GetByImageByNotExistImage()
        {
            IEnumerable<TempCloudImage> lstSelectImage = service.GetByImageId<TempCloudImage>("undefined");
            Assert.Empty(lstSelectImage);
        }

        [Fact]
        public void GetByImageOneImage()
        {
            IEnumerable<TempCloudImage> lstSelectImage = service.GetByImageId<TempCloudImage>(SecondTestImageId);
            Assert.Single(lstSelectImage);
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
                repository.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString());

            ApplicationDbContext dbContext = new ApplicationDbContext(options.Options);

            repository = new EfRepository<TempCloudImage>(dbContext);

            service = new TempCloudImagesService(repository);
        }

        private async void AddTestData()
        {
            await repository.AddAsync(
                  new TempCloudImage()
                  {
                      ImageId = FirstTestImageId,
                      ImageType = (int)ImageType.Original,
                      ImageUrl = "undefine",
                      FileId = "undefine",
                  });

            await repository.AddAsync(
                  new TempCloudImage()
                  {
                      ImageId = FirstTestImageId,
                      ImageType = (int)ImageType.Thumbnail,
                      ImageUrl = "undefine",
                      FileId = "undefine",
                  });

            await repository.AddAsync(
                  new TempCloudImage()
                  {
                      ImageId = SecondTestImageId,
                      ImageType = (int)ImageType.Original,
                      ImageUrl = "undefine",
                      FileId = "undefine",
                  });

            await repository.AddAsync(
                  new TempCloudImage()
                  {
                      ImageId = ThirdTestImageId,
                      ImageType = (int)ImageType.Thumbnail,
                      ImageUrl = "undefine",
                      FileId = "undefine",
                  });

            await repository.SaveChangesAsync();
        }
    }
}
