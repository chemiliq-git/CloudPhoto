namespace CloudPhoto.Web.Tests.Controllers
{
    using System.Collections.Generic;

    using CloudPhoto.Common;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Web.Controllers;
    using CloudPhoto.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Xunit;

    public class HomeControllerTest
    {
        private const string FirstCategoryId = "firstCategoryId";
        private const string SecondCategoryId = "secondCategoryId";

        private const string FirstImageId = "firstImageId";
        private const string SecondImageId = "secondImageId";
        private const string ThirdImageId = "thirdImageId";
        private const string FourImageId = "fourImageId";

        [Theory]
        [InlineData(null)]
        [InlineData("NoDigit")]
        [InlineData("2")]
        public void CheckConfigCategoryCountSettings(string showMostLikeCategoryCount)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(showMostLikeCategoryCount))
            {
                settings.Add(GlobalConstants.ShowMostLikeCategoryCount, showMostLikeCategoryCount);
            }

            settings.Add(GlobalConstants.ShowMostLikeImageByCategoryCount, "2");

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var mockCategoryService = new Mock<ICategoriesService>();
            mockCategoryService.Setup(x => x.GetMostLikedCategory<CategoryHomeViewModel>(2))
                 .Returns(new List<CategoryHomeViewModel>()
                 {
                     new CategoryHomeViewModel()
                     {
                          Id = FirstCategoryId,
                          Name = "TestCategory",
                     },
                     new CategoryHomeViewModel()
                     {
                          Id = SecondCategoryId,
                          Name = "TestCategory",
                     },
                 });

            var mockImageService = new Mock<IImagesService>();
            mockImageService.Setup(x => x.GetMostLikeImageByCategory<ImageHomeViewModel>(FirstCategoryId, 2))
                .Returns(new List<ImageHomeViewModel>()
                {
                    new ImageHomeViewModel()
                    {
                        Id = FirstImageId,
                        ThumbnailImageUrl = "imageUrl1",
                    },
                    new ImageHomeViewModel()
                    {
                        Id = SecondImageId,
                        ThumbnailImageUrl = "imageUrl2",
                    },
                });

            mockImageService.Setup(x => x.GetMostLikeImageByCategory<ImageHomeViewModel>(SecondCategoryId, 2))
                .Returns(new List<ImageHomeViewModel>()
                {
                    new ImageHomeViewModel()
                    {
                        Id = FirstImageId,
                        ThumbnailImageUrl = "imageUrl1",
                    },
                    new ImageHomeViewModel()
                    {
                        Id = SecondImageId,
                        ThumbnailImageUrl = "imageUrl2",
                    },
                });

            HomeController controller = new HomeController(
                configuration,
                mockCategoryService.Object,
                mockImageService.Object);

            IActionResult result = controller.Index();
            Assert.IsType<ViewResult>(result);

            ViewResult viewResult = (ViewResult)result;
            Assert.IsType<List<HomeIndexViewModel>>(viewResult.Model);

            List<HomeIndexViewModel> data = (List<HomeIndexViewModel>)viewResult.Model;
            Assert.Equal(2, data.Count);

            Assert.Equal(2, data[0].CategoryImages.Count);
            Assert.Equal(2, data[1].CategoryImages.Count);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("NoDigit")]
        [InlineData("4")]
        public void CheckConfigImageCountSettings(string showMostLikeImageByCategoryCount)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>
            {
                { GlobalConstants.ShowMostLikeCategoryCount, "1" },
            };

            if (!string.IsNullOrEmpty(showMostLikeImageByCategoryCount))
            {
                settings.Add(GlobalConstants.ShowMostLikeImageByCategoryCount, showMostLikeImageByCategoryCount);
            }

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var mockCategoryService = new Mock<ICategoriesService>();
            mockCategoryService.Setup(x => x.GetMostLikedCategory<CategoryHomeViewModel>(1))
                 .Returns(new List<CategoryHomeViewModel>()
                 {
                     new CategoryHomeViewModel()
                     {
                          Id = FirstCategoryId,
                          Name = "TestCategory",
                     },
                 });

            var mockImageService = new Mock<IImagesService>();
            mockImageService.Setup(x => x.GetMostLikeImageByCategory<ImageHomeViewModel>(FirstCategoryId, 4))
                .Returns(new List<ImageHomeViewModel>()
                {
                    new ImageHomeViewModel()
                    {
                        Id = FirstImageId,
                        ThumbnailImageUrl = "imageUrl1",
                    },
                    new ImageHomeViewModel()
                    {
                        Id = SecondImageId,
                        ThumbnailImageUrl = "imageUrl2",
                    },
                    new ImageHomeViewModel()
                    {
                        Id = ThirdImageId,
                        ThumbnailImageUrl = "imageUrl2",
                    },
                    new ImageHomeViewModel()
                    {
                        Id = FourImageId,
                        ThumbnailImageUrl = "imageUrl2",
                    },
                });

            HomeController controller = new HomeController(
                configuration,
                mockCategoryService.Object,
                mockImageService.Object);

            IActionResult result = controller.Index();
            Assert.IsType<ViewResult>(result);

            ViewResult viewResult = (ViewResult)result;
            Assert.IsType<List<HomeIndexViewModel>>(viewResult.Model);

            List<HomeIndexViewModel> data = (List<HomeIndexViewModel>)viewResult.Model;
            Assert.Single(data);

            Assert.Equal(4, data[0].CategoryImages.Count);
        }

        [Fact]
        public void CheckResponseIsCorrect()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
            {
                { GlobalConstants.ShowMostLikeCategoryCount, "1" },
                { GlobalConstants.ShowMostLikeImageByCategoryCount, "2" },
            })
                .Build();

            var mockCategoryService = new Mock<ICategoriesService>();
            mockCategoryService.Setup(x => x.GetMostLikedCategory<CategoryHomeViewModel>(1))
                 .Returns(new List<CategoryHomeViewModel>()
                 {
                     new CategoryHomeViewModel()
                     {
                          Id = FirstCategoryId,
                          Name = "TestCategory",
                     },
                 });

            var mockImageService = new Mock<IImagesService>();
            mockImageService.Setup(x => x.GetMostLikeImageByCategory<ImageHomeViewModel>(FirstCategoryId, 2))
                .Returns(new List<ImageHomeViewModel>()
                {
                    new ImageHomeViewModel()
                    {
                        Id = FirstImageId,
                        ThumbnailImageUrl = "imageUrl1",
                    },
                    new ImageHomeViewModel()
                    {
                        Id = SecondImageId,
                        ThumbnailImageUrl = "imageUrl2",
                    },
                });

            HomeController controller = new HomeController(
                configuration,
                mockCategoryService.Object,
                mockImageService.Object);

            IActionResult result = controller.Index();
            Assert.IsType<ViewResult>(result);

            ViewResult viewResult = (ViewResult)result;
            Assert.IsType<List<HomeIndexViewModel>>(viewResult.Model);

            List<HomeIndexViewModel> data = (List<HomeIndexViewModel>)viewResult.Model;
            Assert.Single(data);

            Assert.Equal(2, data[0].CategoryImages.Count);
            Assert.Same(FirstImageId, data[0].CategoryImages[0].Id);
            Assert.Same(FirstCategoryId, data[0].CategoryInfo.Id);
        }
    }
}
