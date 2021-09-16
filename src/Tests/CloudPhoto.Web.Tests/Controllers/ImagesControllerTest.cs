namespace CloudPhoto.Web.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Web.Controllers;
    using CloudPhoto.Web.ViewModels.Images;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class ImagesControllerTest
    {
        private const string FirstTestUserId = "firstTestUserId";

        [Theory]
        [InlineData("Create", new Type[] { typeof(CreateImageViewModel) })]
        [InlineData("GetSearchingData", new Type[] { typeof(int), typeof(int), typeof(string), typeof(string) })]
        [InlineData("GetUserImagesData", new Type[] { typeof(int), typeof(int), typeof(string), typeof(string) })]
        public void CheckControllerDecoratedWithValidateAntiForgeryToken(string methodName, Type[] parameterType)
        {
            var mockImagesService = new Mock<IImagesService>();
            var mockCategoryService = new Mock<ICategoriesService>();
            UserManager<ApplicationUser> userManager = MockUserManager();
            var logger = new Mock<ILogger<ImagesController>>();

            ImagesController controller = new ImagesController(
             mockImagesService.Object,
             mockCategoryService.Object,
             userManager,
             logger.Object);

            var type = controller.GetType();
            var methodInfo = type.GetMethod(methodName, parameterType);
            var attributes = methodInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true);
            Assert.True(attributes.Any(), $"No ValidateAntiForgeryTokenAttribute found on {methodName} method");
        }

        [Theory]
        [InlineData(0, 6)]
        [InlineData(1, 0)]
        public async void GetSearchingDataValidateParameters(int pageIndex, int pageSize)
        {
            var mockImagesService = new Mock<IImagesService>();

            var mockCategoryService = new Mock<ICategoriesService>();
            UserManager<ApplicationUser> userManager = MockUserManager();
            var logger = new Mock<ILogger<ImagesController>>();

            ImagesController controller = new ImagesController(
             mockImagesService.Object,
             mockCategoryService.Object,
             userManager,
             logger.Object).WithIdentity(FirstTestUserId, "TestIdentity");

            ActionResult result = await controller.GetSearchingData(pageIndex, pageSize, null, null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void GetSearchDataMissingImages()
        {
            var mockImagesService = new Mock<IImagesService>();

            var mockCategoryService = new Mock<ICategoriesService>();
            UserManager<ApplicationUser> userManager = MockUserManager();
            var logger = new Mock<ILogger<ImagesController>>();

            ImagesController controller = new ImagesController(
             mockImagesService.Object,
             mockCategoryService.Object,
             userManager,
             logger.Object).WithIdentity(FirstTestUserId, "TestIdentity");

            ActionResult result = await controller.GetSearchingData(1, 1, null, null);
            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async void GetSearchingData()
        {
            int pageIndex = 1;
            int pageSize = 6;
            string searchText = null;
            string selectCategory = null;

            SearchImageData localSearchData = new SearchImageData
            {
                FilterByTag = searchText,
                LikeForUserId = FirstTestUserId,
            };

            var mockImagesService = new Mock<IImagesService>();
            mockImagesService.Setup(x => x.GetByFilter<ListImageViewModel>(
               It.Is<SearchImageData>(x => x.LikeForUserId == FirstTestUserId && x.FilterByTag == searchText),
               pageSize,
               pageIndex))
                   .Returns(new List<ListImageViewModel>()
                   {
                       new ListImageViewModel()
                       {
                           Id = "1",
                           Title = "ImageTitle",
                       },
                       new ListImageViewModel()
                       {
                           Id = "2",
                           Title = "ImageTitle",
                       },
                   });

            var mockCategoryService = new Mock<ICategoriesService>();
            UserManager<ApplicationUser> userManager = MockUserManager();
            var logger = new Mock<ILogger<ImagesController>>();

            ImagesController controller = new ImagesController(
             mockImagesService.Object,
             mockCategoryService.Object,
             userManager,
             logger.Object).WithIdentity(FirstTestUserId, "TestIdentity");

            ActionResult result = await controller.GetSearchingData(pageIndex, pageSize, searchText, selectCategory);
            Assert.IsType<PartialViewResult>(result);

            PartialViewResult viewResult = (PartialViewResult)result;
            Assert.IsType<List<ListImageViewModel>>(viewResult.Model);

            List<ListImageViewModel> model = (List<ListImageViewModel>)viewResult.Model;
            Assert.Equal(2, model.Count);

            for (int index = 0; index < model.Count; index++)
            {
                Assert.Equal(model[index].ImageIndex, index + 1);
            }
        }

        [Theory]
        [InlineData(0, 1, FirstTestUserId, "uploads")]
        [InlineData(1, 0, FirstTestUserId, "uploads")]
        [InlineData(1, 1, "NotExistUser", "uploads")]
        [InlineData(1, 1, null, "uploads")]
        [InlineData(1, 1, FirstTestUserId, null)]
        [InlineData(1, 1, FirstTestUserId, "NotExistType")]
        public async void GetUserImagesDataValidateParameters(int pageIndex, int pageSize, string userId, string type)
        {
            var mockImagesService = new Mock<IImagesService>();

            var mockCategoryService = new Mock<ICategoriesService>();
            UserManager<ApplicationUser> userManager = MockUserManager();
            var logger = new Mock<ILogger<ImagesController>>();

            ImagesController controller = new ImagesController(
             mockImagesService.Object,
             mockCategoryService.Object,
             userManager,
             logger.Object).WithIdentity(FirstTestUserId, "TestIdentity");

            IActionResult result = await controller.GetUserImagesData(pageIndex, pageSize, userId, type);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void GetUserImagesDataMissingImages()
        {
            var mockImagesService = new Mock<IImagesService>();

            var mockCategoryService = new Mock<ICategoriesService>();
            UserManager<ApplicationUser> userManager = MockUserManager();
            var logger = new Mock<ILogger<ImagesController>>();

            ImagesController controller = new ImagesController(
             mockImagesService.Object,
             mockCategoryService.Object,
             userManager,
             logger.Object).WithIdentity(FirstTestUserId, "TestIdentity");

            IActionResult result = await controller.GetUserImagesData(1, 1, FirstTestUserId, "uploads");
            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async void GetUserImagesData()
        {
            int pageIndex = 1;
            int pageSize = 6;
            string searchText = null;

            SearchImageData localSearchData = new SearchImageData
            {
                FilterByTag = searchText,
                LikeForUserId = FirstTestUserId,
            };

            var mockImagesService = new Mock<IImagesService>();
            mockImagesService.Setup(x => x.GetByFilter<ListImageViewModel>(
               It.Is<SearchImageData>(x => x.LikeForUserId == FirstTestUserId && x.AuthorId == FirstTestUserId),
               pageSize,
               pageIndex))
                   .Returns(new List<ListImageViewModel>()
                   {
                       new ListImageViewModel()
                       {
                           Id = "1",
                           Title = "ImageTitle",
                       },
                       new ListImageViewModel()
                       {
                           Id = "2",
                           Title = "ImageTitle",
                       },
                   });

            var mockCategoryService = new Mock<ICategoriesService>();
            UserManager<ApplicationUser> userManager = MockUserManager();
            var logger = new Mock<ILogger<ImagesController>>();

            ImagesController controller = new ImagesController(
             mockImagesService.Object,
             mockCategoryService.Object,
             userManager,
             logger.Object).WithIdentity(FirstTestUserId, "TestIdentity");

            IActionResult result = await controller.GetUserImagesData(pageIndex, pageSize, FirstTestUserId, "uploads");
            Assert.IsType<PartialViewResult>(result);

            PartialViewResult viewResult = (PartialViewResult)result;
            Assert.IsType<List<ListImageViewModel>>(viewResult.Model);

            List<ListImageViewModel> model = (List<ListImageViewModel>)viewResult.Model;
            Assert.Equal(2, model.Count);

            for (int index = 0; index < model.Count; index++)
            {
                Assert.Equal(model[index].ImageIndex, index + 1);
            }
        }

        private static UserManager<ApplicationUser> MockUserManager()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            userStore.Setup(x => x.FindByIdAsync(FirstTestUserId, CancellationToken.None))
                .ReturnsAsync(new ApplicationUser()
                {
                    Id = FirstTestUserId,
                    FirstName = "Test1",
                    LastName = "User1",
                });

            userStore.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>(), CancellationToken.None))
                .ReturnsAsync(IdentityResult.Success);

            var userManagerMock = new UserManager<ApplicationUser>(userStore.Object, null, null, null, null, null, null, null, null);
            return userManagerMock;
        }
    }
}
