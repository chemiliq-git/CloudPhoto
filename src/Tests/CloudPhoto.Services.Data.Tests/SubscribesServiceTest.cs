namespace CloudPhoto.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using SubscribesService;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class SubscribesServiceTest : IDisposable
    {
        private const string FirstTestUserId = "firstTestUserId";
        private const string SecondTestUserId = "secondTestUserId";
        private const string ThirdTestUserId = "thirdTestUserId";

        private IDeletableEntityRepository<UserSubscribe> userSubscribeRepository;
        private SubscribesService subscribesService;

        public SubscribesServiceTest()
        {
            InitTestServices();

            AddTestData();
        }

        [Fact]
        public void GetSubscribes()
        {
            IEnumerable<UserSubscribe> selectSubscribes = subscribesService.GetSubscribes<UserSubscribe>(FirstTestUserId);
            Assert.Equal(2, selectSubscribes?.Count());
        }

        [Fact]
        public void GetSubscribesWhenNotExist()
        {
            IEnumerable<UserSubscribe> selectSubscribes = subscribesService.GetSubscribes<UserSubscribe>(ThirdTestUserId);
            Assert.Empty(selectSubscribes);
        }

        [Fact]
        public void CheckHasSubscribeToUserShouldReturnTrue()
        {
            IEnumerable<UserSubscribe> selectSubscribes = subscribesService.GetSubscribes<UserSubscribe>(FirstTestUserId, SecondTestUserId);
            Assert.Equal(1, selectSubscribes?.Count());
        }

        [Fact]
        public void CheckHasSubscribeToUserShouldReturnFalse()
        {
            IEnumerable<UserSubscribe> selectSubscribes = subscribesService.GetSubscribes<UserSubscribe>(ThirdTestUserId, SecondTestUserId);
            Assert.Empty(selectSubscribes);
        }

        [Fact]
        public async void SubscribeToUser()
        {
            bool result = await subscribesService.ManageUserSubsctibe(SecondTestUserId, FirstTestUserId, true);
            Assert.True(result);
        }

        [Fact]
        public async void UnsubscribeFromUser()
        {
            bool result = await subscribesService.ManageUserSubsctibe(SecondTestUserId, ThirdTestUserId, false);
            Assert.True(result);
        }

        [Fact]
        public async void UnSubscribeToUserShouldFalse()
        {
            bool result = await subscribesService.ManageUserSubsctibe(ThirdTestUserId, FirstTestUserId, false);
            Assert.False(result);
        }

        [Fact]
        public async void SubscribeToUserShouldFalse()
        {
            bool result = await subscribesService.ManageUserSubsctibe(FirstTestUserId, SecondTestUserId, true);
            Assert.False(result);
        }

        [Fact]
        public async void SubscribeToYourself()
        {
            bool result = await subscribesService.ManageUserSubsctibe(FirstTestUserId, FirstTestUserId, true);
            Assert.False(result);
        }

        [Fact]
        public async void UnSubscribeToYourself()
        {
            bool result = await subscribesService.ManageUserSubsctibe(FirstTestUserId, FirstTestUserId, false);
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
                userSubscribeRepository.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString());

            ApplicationDbContext dbContext = new ApplicationDbContext(options.Options);

            userSubscribeRepository = new EfDeletableEntityRepository<UserSubscribe>(dbContext);

            var logger = Mock.Of<ILogger<SubscribesService>>();

            subscribesService = new SubscribesService(
                logger,
                userSubscribeRepository);
        }

        private async void AddTestData()
        {
            await userSubscribeRepository.AddAsync(
                new UserSubscribe()
                {
                    UserSubscribedId = FirstTestUserId,
                    SubscribeToUserId = ThirdTestUserId,
                });
            await userSubscribeRepository.AddAsync(
                new UserSubscribe()
                {
                    UserSubscribedId = FirstTestUserId,
                    SubscribeToUserId = SecondTestUserId,
                });
            await userSubscribeRepository.AddAsync(
               new UserSubscribe()
               {
                   UserSubscribedId = SecondTestUserId,
                   SubscribeToUserId = ThirdTestUserId,
               });
            await userSubscribeRepository.SaveChangesAsync();
        }
    }
}
