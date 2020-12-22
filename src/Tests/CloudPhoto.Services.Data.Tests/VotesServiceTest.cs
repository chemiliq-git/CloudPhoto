namespace CloudPhoto.Services.Data.Tests
{
    using System;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using CloudPhoto.Services.Data.VotesService;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class VotesServiceTest : IDisposable
    {
        private const string FirstTestImageId = "firstImageId";
        private const string SecondTestImageId = "secondImageId";
        private const string ThirdTestImageId = "thirdIMageId";
        private const string FourTestImageId = "fourIMageId";

        private const string FirstUserTestId = "firstUserId";
        private const string SecondUserTestId = "secondUserId";
        private const string ThirdUserTestId = "thirdUserId";
        private const string FourUserTestId = "fourUserId";

        private VotesService votesService;
        private EfRepository<Vote> repository;

        public VotesServiceTest()
        {
            this.InitTestServices();

            this.AddTestData();
        }

        [Fact]
        public async void VoteFirstTimeShouldSucceeded()
        {
            bool result = await this.votesService.VoteAsync(ThirdTestImageId, ThirdUserTestId, true);
            Assert.True(result);
        }

        [Fact]
        public async void TryDownVoteFirstTimeShouldFalse()
        {
            bool result = await this.votesService.VoteAsync(FourTestImageId, FourUserTestId, false);
            Assert.False(result);
        }

        [Fact]
        public async void TryVoteManyTimes()
        {
            bool result = await this.votesService.VoteAsync(FirstTestImageId, FirstUserTestId, true);
            Assert.False(result);
        }

        [Fact]
        public async void TryDownVote()
        {
            bool result = await this.votesService.VoteAsync(FirstTestImageId, FirstUserTestId, false);
            Assert.True(result);
        }

        [Fact]
        public async void TryDownVoteWhenAlreadyDownVote()
        {
            bool result = await this.votesService.VoteAsync(SecondTestImageId, FirstUserTestId, false);
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
                this.repository.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            this.repository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));

            var logger = Mock.Of<ILogger<VotesService>>();

            this.votesService = new VotesService(
                logger,
                this.repository);
        }

        private async void AddTestData()
        {
            // FirstTestImageId vote by FirstUserTestId, SecondUserTestId
            // SecondTestImageId vote by FirstUserTestId
            await this.repository.AddAsync(
                 new Vote()
                 {
                     ImageId = FirstTestImageId,
                     AuthorId = FirstUserTestId,
                     IsLike = 1,
                 });
            await this.repository.AddAsync(
                new Vote()
                {
                    ImageId = FirstTestImageId,
                    AuthorId = SecondUserTestId,
                    IsLike = 1,
                });
            await this.repository.AddAsync(
                new Vote()
                {
                    ImageId = SecondTestImageId,
                    AuthorId = FirstUserTestId,
                    IsLike = 0,
                });

            await this.repository.SaveChangesAsync();
        }
    }
}
