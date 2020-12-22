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

    public class VotesServiceTest
    {
        [Fact]
        public async void VoteFirstTimeShouldSucceeded()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var repository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));

            var logger = Mock.Of<ILogger<VotesService>>();

            var voteService = new VotesService(
                logger,
                repository);

            bool result = await voteService.VoteAsync("memoryImageId", "memoryUserId", true);

            Assert.True(result);
        }

        [Fact]
        public async void VoteUnsibscribeWhenNotExitsShouldNoSucceeded()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var repository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));

            var logger = Mock.Of<ILogger<VotesService>>();

            var voteService = new VotesService(
                logger,
                repository);

            bool result = await voteService.VoteAsync("memoryImageId", "memoryUserId", false);

            Assert.False(result);
        }
    }
}
