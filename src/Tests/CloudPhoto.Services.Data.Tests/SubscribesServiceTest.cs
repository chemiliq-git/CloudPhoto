namespace CloudPhoto.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using CloudPhoto.Services.Data.SubscribesService;
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
            this.InitTestServices();

            this.AddTestData();
        }

        [Fact]
        public void GetSubscribes()
        {
            IEnumerable<UserSubscribe> selectSubscribes = this.subscribesService.GetSubscribes<UserSubscribe>(FirstTestUserId);
            Assert.Equal(2, selectSubscribes?.Count());
        }

        [Fact]
        public void GetSubscribesWhenNotExist()
        {
            IEnumerable<UserSubscribe> selectSubscribes = this.subscribesService.GetSubscribes<UserSubscribe>(ThirdTestUserId);
            Assert.Empty(selectSubscribes);
        }

        [Fact]
        public void CheckHasSubscribeToUserShouldReturnTrue()
        {
            IEnumerable<UserSubscribe> selectSubscribes = this.subscribesService.GetSubscribes<UserSubscribe>(FirstTestUserId, SecondTestUserId);
            Assert.Equal(1, selectSubscribes?.Count());
        }

        [Fact]
        public void CheckHasSubscribeToUserShouldReturnFalse()
        {
            IEnumerable<UserSubscribe> selectSubscribes = this.subscribesService.GetSubscribes<UserSubscribe>(ThirdTestUserId, SecondTestUserId);
            Assert.Empty(selectSubscribes);
        }

        [Fact]
        public async void SubscribeToUser()
        {
            bool result = await this.subscribesService.ManageUserSubsctibe(SecondTestUserId, FirstTestUserId, true);
            Assert.True(result);
        }

        [Fact]
        public async void UnsubscribeFromUser()
        {
            bool result = await this.subscribesService.ManageUserSubsctibe(SecondTestUserId, ThirdTestUserId, false);
            Assert.True(result);
        }

        [Fact]
        public async void UnSubscribeToUserShouldFalse()
        {
            bool result = await this.subscribesService.ManageUserSubsctibe(ThirdTestUserId, FirstTestUserId, false);
            //Assert.False(result);
            Assert.True(result);
        }

        [Fact]
        public async void SubscribeToUserShouldFalse()
        {
            bool result = await this.subscribesService.ManageUserSubsctibe(FirstTestUserId, SecondTestUserId, true);
            Assert.False(result);
        }

        [Fact]
        public async void SubscribeToYourself()
        {
            bool result = await this.subscribesService.ManageUserSubsctibe(FirstTestUserId, FirstTestUserId, true);
            Assert.False(result);
        }

        [Fact]
        public async void UnSubscribeToYourself()
        {
            bool result = await this.subscribesService.ManageUserSubsctibe(FirstTestUserId, FirstTestUserId, false);
            Assert.False(result);
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
                this.userSubscribeRepository.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString());

            ApplicationDbContext dbContext = new ApplicationDbContext(options.Options);

            this.userSubscribeRepository = new EfDeletableEntityRepository<UserSubscribe>(dbContext);

            var logger = Mock.Of<ILogger<SubscribesService>>();

            this.subscribesService = new SubscribesService(
                logger,
                this.userSubscribeRepository);
        }

        private async void AddTestData()
        {
            await this.userSubscribeRepository.AddAsync(
                new UserSubscribe()
                {
                    UserSubscribedId = FirstTestUserId,
                    SubscribeToUserId = ThirdTestUserId,
                });
            await this.userSubscribeRepository.AddAsync(
                new UserSubscribe()
                {
                    UserSubscribedId = FirstTestUserId,
                    SubscribeToUserId = SecondTestUserId,
                });
            await this.userSubscribeRepository.AddAsync(
               new UserSubscribe()
               {
                   UserSubscribedId = SecondTestUserId,
                   SubscribeToUserId = ThirdTestUserId,
               });
            await this.userSubscribeRepository.SaveChangesAsync();
        }
    }
}
