namespace CloudPhoto.Web.Tests.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;

    using Data.Models;
    using CloudPhoto.Services.Data.SubscribesService;
    using CloudPhoto.Web.Controllers;
    using ViewModels.Subscribes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class SubscribesControllerTest
    {
        private const string FirstTestUserId = "firstTestUserId";
        private const string SecondTestUserId = "secondTestUserId";

        [Fact]
        public void CheckControllerDecoratedWithAuthorizeAttribute()
        {
            UserManager<ApplicationUser> userManager = MockUserManager();

            var mockSubscribesService = new Mock<ISubscribesService>();

            SubscribesController controller = new SubscribesController(
             userManager,
             mockSubscribesService.Object).WithIdentity(FirstTestUserId, "TestUser");

            var type = controller.GetType();
            var methodInfo = type.GetMethod("Subscribe", new Type[] { typeof(SubscribeInputModel) });
            var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            Assert.True(attributes.Any(), "No AuthorizeAttribute found on Index method");
        }

        [Fact]
        public void CheckControllerDecoratedWithValidateAntiForgeryToken()
        {
            UserManager<ApplicationUser> userManager = MockUserManager();

            var mockSubscribesService = new Mock<ISubscribesService>();

            SubscribesController controller = new SubscribesController(
             userManager,
             mockSubscribesService.Object).WithIdentity(FirstTestUserId, "TestUser");

            var type = controller.GetType();
            var methodInfo = type.GetMethod("Subscribe", new Type[] { typeof(SubscribeInputModel) });
            var attributes = methodInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true);
            Assert.True(attributes.Any(), "No ValidateAntiForgeryTokenAttribute found on Index method");
        }

        [Fact]
        public async void ValidateInputModel()
        {
            UserManager<ApplicationUser> userManager = MockUserManager();

            var mockSubscribesService = new Mock<ISubscribesService>();

            SubscribesController controller = new SubscribesController(
             userManager,
             mockSubscribesService.Object).WithIdentity(FirstTestUserId, "TestUser");

            ActionResult<SubscribeResponseModel> result = await controller.Subscribe(null);
            Assert.IsType<BadRequestResult>(result.Result);

            result = await controller.Subscribe(new SubscribeInputModel()
            {
                UserId = null,
                Follow = true,
            });
            Assert.IsType<BadRequestResult>(result.Result);

            result = await controller.Subscribe(new SubscribeInputModel()
            {
                UserId = FirstTestUserId,
                Follow = true,
            });
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async void TestSubscribeFunctionality()
        {
            UserManager<ApplicationUser> userManager = MockUserManager();

            var mockSubscribesService = new Mock<ISubscribesService>();
            mockSubscribesService.Setup(x => x.ManageUserSubsctibe(FirstTestUserId, SecondTestUserId, true))
                .ReturnsAsync(true);
            mockSubscribesService.Setup(x => x.ManageUserSubsctibe(FirstTestUserId, SecondTestUserId, false))
                .ReturnsAsync(false);

            SubscribesController controller = new SubscribesController(
             userManager,
             mockSubscribesService.Object).WithIdentity(FirstTestUserId, "TestUser");

            ActionResult<SubscribeResponseModel> result = await controller.Subscribe(null);
            Assert.IsType<BadRequestResult>(result.Result);

            result = await controller.Subscribe(new SubscribeInputModel()
            {
                UserId = SecondTestUserId,
                Follow = true,
            });
            Assert.True(result.Value.Result);

            result = await controller.Subscribe(new SubscribeInputModel()
            {
                UserId = SecondTestUserId,
                Follow = false,
            });
            Assert.False(result.Value.Result);
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

            userStore.Setup(x => x.FindByIdAsync(SecondTestUserId, CancellationToken.None))
                .ReturnsAsync(new ApplicationUser()
                {
                    Id = SecondTestUserId,
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
