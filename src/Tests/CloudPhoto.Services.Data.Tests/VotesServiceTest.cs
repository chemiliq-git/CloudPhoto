namespace CloudPhoto.Services.Data.Tests
{
    using System;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using VotesService;
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
            InitTestServices();

            AddTestData();
        }

        [Fact]
        public async void VoteFirstTimeShouldSucceeded()
        {
            bool result = await votesService.VoteAsync(ThirdTestImageId, ThirdUserTestId, true);
            Assert.True(result);
        }

        [Fact]
        public async void TryDownVoteFirstTimeShouldFalse()
        {
            bool result = await votesService.VoteAsync(FourTestImageId, FourUserTestId, false);
            Assert.False(result);
        }

        [Fact]
        public async void TryVoteManyTimes()
        {
            bool result = await votesService.VoteAsync(FirstTestImageId, FirstUserTestId, true);
            Assert.False(result);
        }

        [Fact]
        public async void TryDownVote()
        {
            bool result = await votesService.VoteAsync(FirstTestImageId, FirstUserTestId, false);
            Assert.True(result);
        }

        [Fact]
        public async void TryDownVoteWhenAlreadyDownVote()
        {
            bool result = await votesService.VoteAsync(SecondTestImageId, FirstUserTestId, false);
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
                repository.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            repository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));

            var logger = Mock.Of<ILogger<VotesService>>();

            votesService = new VotesService(
                logger,
                repository);
        }

        private async void AddTestData()
        {
            // FirstTestImageId vote by FirstUserTestId, SecondUserTestId
            // SecondTestImageId vote by FirstUserTestId
            await repository.AddAsync(
                 new Vote()
                 {
                     ImageId = FirstTestImageId,
                     AuthorId = FirstUserTestId,
                     IsLike = 1,
                 });
            await repository.AddAsync(
                new Vote()
                {
                    ImageId = FirstTestImageId,
                    AuthorId = SecondUserTestId,
                    IsLike = 1,
                });
            await repository.AddAsync(
                new Vote()
                {
                    ImageId = SecondTestImageId,
                    AuthorId = FirstUserTestId,
                    IsLike = 0,
                });

            await repository.SaveChangesAsync();
        }
    }
}
