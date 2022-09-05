namespace CloudPhoto.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using UsersServices;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class UsersServiceTest : IDisposable
    {
        private const string FirstTestUserId = "firstTestUserId";
        private const string SecondTestUserId = "secondTestUserId";
        private const string ThirdTestUserId = "thirdTestUserId";

        private EfDeletableEntityRepository<ApplicationUser> userRepository;
        private EfRepository<UserSubscribe> userSubscribeRepository;
        private UserManager<ApplicationUser> userManager;
        private UsersService usersService;

        public UsersServiceTest()
        {
            InitTestServices();

            AddTestData();
        }

        [Fact]
        public void GetUserInfoForNotExistUser()
        {
            ApplicationUser applicationUser = usersService.GetUserInfo<ApplicationUser>("notExistUserId", null);
            Assert.Null(applicationUser);
        }

        [Fact]
        public void GetUserInfoForExistUser()
        {
            ApplicationUser applicationUser = usersService.GetUserInfo<ApplicationUser>(FirstTestUserId, null);
            Assert.Equal(FirstTestUserId, applicationUser?.Id);
        }

        [Fact]
        public void GetUserCheckFollowings()
        {
            ApplicationUser applicationUser = usersService.GetUserInfo<ApplicationUser>(FirstTestUserId, null);
            Assert.Equal(2, applicationUser.CountFollowing);
        }

        [Fact]
        public void GetUserCheckFollowers()
        {
            ApplicationUser applicationUser = usersService.GetUserInfo<ApplicationUser>(FirstTestUserId, null);
            Assert.Equal(1, applicationUser.CountFollowers);
        }

        [Fact]
        public void GetUserCheckSubscribeShouldTrue()
        {
            ApplicationUser applicationUser = usersService.GetUserInfo<ApplicationUser>(FirstTestUserId, SecondTestUserId);
            Assert.True(applicationUser.IsFollowCurrentUser);
        }

        [Fact]
        public void GetUserCheckSubscribeShouldFalse()
        {
            ApplicationUser applicationUser = usersService.GetUserInfo<ApplicationUser>(FirstTestUserId, ThirdTestUserId);
            Assert.False(applicationUser.IsFollowCurrentUser);
        }

        [Fact]
        public void GetFollowers()
        {
            IEnumerable<ApplicationUser> lstFollowerUsers
                = usersService.GetFollowerUsers<ApplicationUser>(FirstTestUserId, SecondTestUserId, 0, 0);
            Assert.Single(lstFollowerUsers.ToList());
        }

        [Fact]
        public void GetFollowersMissingUserIdInputData()
        {
            IEnumerable<ApplicationUser> lstFollowerUsers
                = usersService.GetFollowerUsers<ApplicationUser>(null, SecondTestUserId, 0, 0);
            Assert.Empty(lstFollowerUsers.ToList());
        }

        [Fact]
        public void GetFollowersMissingCurrentUserIdInputData()
        {
            IEnumerable<ApplicationUser> lstFollowerUsers
                = usersService.GetFollowerUsers<ApplicationUser>(FirstTestUserId, null, 0, 0);
            Assert.Single(lstFollowerUsers.ToList());
        }

        [Fact]
        public void GetFollowersPagingTest()
        {
            // try get not exist index of list
            IEnumerable<ApplicationUser> lstFollowerUsers
                = usersService.GetFollowerUsers<ApplicationUser>(FirstTestUserId, null, 5, 5);
            Assert.Empty(lstFollowerUsers.ToList());
        }

        [Fact]
        public void GetFollowings()
        {
            IEnumerable<ApplicationUser> lstFollowerUsers
                = usersService.GetFollowingUsers<ApplicationUser>(FirstTestUserId, SecondTestUserId, 0, 0);
            Assert.Equal(2, lstFollowerUsers.ToList()?.Count);
        }

        [Fact]
        public void GetFollowingsPagingTest()
        {
            IEnumerable<ApplicationUser> lstFollowerUsers
                = usersService.GetFollowingUsers<ApplicationUser>(FirstTestUserId, SecondTestUserId, 1, 1);
            Assert.Single(lstFollowerUsers.ToList());
        }

        [Fact]
        public void GetFollowingMissingUserIdInputData()
        {
            IEnumerable<ApplicationUser> lstFollowerUsers
                = usersService.GetFollowingUsers<ApplicationUser>(null, SecondTestUserId, 0, 0);
            Assert.Empty(lstFollowerUsers.ToList());
        }

        [Fact]
        public void GetFollowingMissingCurrentUserIdInputData()
        {
            IEnumerable<ApplicationUser> lstFollowerUsers
                = usersService.GetFollowingUsers<ApplicationUser>(FirstTestUserId, null, 0, 0);
            Assert.Equal(2, lstFollowerUsers.ToList()?.Count);
        }

        [Fact]
        public async void ChangeAvatar()
        {
            bool result = await usersService.ChangeAvatar(FirstTestUserId, "testAvatar");
            Assert.True(result);
        }

        [Fact]
        public async void ChangeAvatarMissingUserId()
        {
            bool result = await usersService.ChangeAvatar(null, "testAvatar");
            Assert.False(result);
        }

        [Fact]
        public async void ChangeAvatarMissingAvatarUrl()
        {
            bool result = await usersService.ChangeAvatar(FirstTestUserId, null);
            Assert.False(result);
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
                userRepository.Dispose();
                userSubscribeRepository.Dispose();

                userManager.Dispose();
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

            userStore.Setup(x => x.FindByIdAsync(SecondTestUserId, CancellationToken.None))
                .ReturnsAsync(new ApplicationUser()
                {
                    Id = SecondTestUserId,
                    FirstName = "Test2",
                    LastName = "User2",
                });

            userStore.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>(), CancellationToken.None))
                .ReturnsAsync(IdentityResult.Success);

            var userManagerMock = new UserManager<ApplicationUser>(userStore.Object, null, null, null, null, null, null, null, null);
            return userManagerMock;
        }

        private void InitTestServices()
        {
            UserManager<ApplicationUser> userManagerMock = MockUserManager();
            userManager = userManagerMock;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            ApplicationDbContext dbContext = new ApplicationDbContext(options.Options);

            userRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            userSubscribeRepository = new EfRepository<UserSubscribe>(dbContext);

            var logger = Mock.Of<ILogger<UsersService>>();

            usersService = new UsersService(
                logger,
                userManager,
                userRepository,
                userSubscribeRepository);
        }

        private async void AddTestData()
        {
            ApplicationUser firstUser = new ApplicationUser
            {
                Id = FirstTestUserId,
                FirstName = "Test1",
                LastName = "User1",
            };
            await userRepository.AddAsync(firstUser);

            ApplicationUser secondtUser = new ApplicationUser
            {
                Id = SecondTestUserId,
                FirstName = "Test2",
                LastName = "User2",
            };
            await userRepository.AddAsync(secondtUser);

            ApplicationUser thirdtUser = new ApplicationUser
            {
                Id = ThirdTestUserId,
                FirstName = "Test3",
                LastName = "User3",
            };
            await userRepository.AddAsync(thirdtUser);

            await userRepository.SaveChangesAsync();

            // add User subscribers
            await userSubscribeRepository.AddAsync(
                new UserSubscribe() { UserSubscribedId = FirstTestUserId, SubscribeToUserId = SecondTestUserId });
            await userSubscribeRepository.AddAsync(
                new UserSubscribe() { UserSubscribedId = FirstTestUserId, SubscribeToUserId = ThirdTestUserId });
            await userSubscribeRepository.AddAsync(
                new UserSubscribe() { UserSubscribedId = SecondTestUserId, SubscribeToUserId = FirstTestUserId });

            await userSubscribeRepository.SaveChangesAsync();
        }
    }
}
