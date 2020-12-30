namespace CloudPhoto.Web.Tests.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;

    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.VotesService;
    using CloudPhoto.Web.Controllers;
    using CloudPhoto.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class VotesControllerTest
    {
        private const string FirstTestImageId = "firstImageId";

        private const string FirstTestUserId = "firstTestUserId";

        [Fact]
        public void CheckVoteControllerDecoratedWithAuthorizeAttribute()
        {
            UserManager<ApplicationUser> userManager = MockUserManager();

            var mockVoteService = new Mock<IVotesService>();
            var mockImageService = new Mock<IImagesService>();

            VotesController controller = new VotesController(
             userManager,
             mockVoteService.Object,
             mockImageService.Object).WithIdentity(FirstTestUserId, "TestUser");

            var type = controller.GetType();
            var methodInfo = type.GetMethod("Index", new Type[] { typeof(VoteInputModel) });
            var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            Assert.True(attributes.Any(), "No AuthorizeAttribute found on Index method");
        }

        [Fact]
        public void CheckVoteControllerDecoratedWithValidateAntiForgeryToken()
        {
            UserManager<ApplicationUser> userManager = MockUserManager();

            var mockVoteService = new Mock<IVotesService>();
            var mockImageService = new Mock<IImagesService>();

            VotesController controller = new VotesController(
             userManager,
             mockVoteService.Object,
             mockImageService.Object).WithIdentity(FirstTestUserId, "TestUser");

            var type = controller.GetType();
            var methodInfo = type.GetMethod("Index", new Type[] { typeof(VoteInputModel) });
            var attributes = methodInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true);
            Assert.True(attributes.Any(), "No ValidateAntiForgeryTokenAttribute found on Index method");
        }

        [Fact]
        public async void ValidateInputModel()
        {
            UserManager<ApplicationUser> userManager = MockUserManager();

            var mockVoteService = new Mock<IVotesService>();
            var mockImageService = new Mock<IImagesService>();

            VotesController controller = new VotesController(
             userManager,
             mockVoteService.Object,
             mockImageService.Object).WithIdentity(FirstTestUserId, "TestUser");

            ActionResult<VoteResponseModel> result = await controller.Index(null);
            Assert.IsType<BadRequestResult>(result.Result);

            result = await controller.Index(new VoteInputModel()
            {
                ImageId = null,
            });
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async void TestVoteFunctionality()
        {
            UserManager<ApplicationUser> userManager = MockUserManager();

            var mockVoteService = new Mock<IVotesService>();
            mockVoteService.Setup(x => x.VoteAsync(VotesControllerTest.FirstTestImageId, It.IsAny<string>(), true))
                .ReturnsAsync(false);
            mockVoteService.Setup(x => x.VoteAsync(VotesControllerTest.FirstTestImageId, It.IsAny<string>(), false))
                .ReturnsAsync(true);

            var mockImageService = new Mock<IImagesService>();
            mockImageService.Setup(x => x.GetImageById<Image>(It.IsAny<string>()))
                .Returns(new Image()
                {
                    Id = VotesControllerTest.FirstTestImageId,
                });

            VotesController controller = new VotesController(
                userManager,
                mockVoteService.Object,
                mockImageService.Object).WithIdentity(FirstTestUserId, "TestUser");

            VoteInputModel model = new VoteInputModel()
            {
                ImageId = VotesControllerTest.FirstTestImageId,
                IsLike = false,
            };

            ActionResult<VoteResponseModel> result = await controller.Index(model);
            Assert.True(result.Value.Result);

            model.IsLike = true;
            result = await controller.Index(model);
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

            userStore.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>(), CancellationToken.None))
                .ReturnsAsync(IdentityResult.Success);

            var userManagerMock = new UserManager<ApplicationUser>(userStore.Object, null, null, null, null, null, null, null, null);
            return userManagerMock;
        }
    }
}
