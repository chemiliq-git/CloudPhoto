namespace CloudPhoto.Services.Data.VotesService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using Mapping;
    using Microsoft.Extensions.Logging;

    public class VotesService : IVotesService
    {
        public VotesService(
            ILogger<VotesService> logger,
            IRepository<Vote> votesRepository)
        {
            Logger = logger;
            VotesRepository = votesRepository;
        }

        public ILogger<VotesService> Logger { get; }

        public IRepository<Vote> VotesRepository { get; }

        public async Task<bool> VoteAsync(string imageId, string userId, bool isLike)
        {
            var vote = VotesRepository.All()
                .FirstOrDefault(x => x.ImageId == imageId && x.AuthorId == userId);
            if (vote != null)
            {
                if (isLike
                    && vote.IsLike == ((int)VoteType.Like))
                {
                    Logger.LogError($"Try to like image that already like. VoteId:{vote.Id}");
                    return false;
                }

                if (!isLike
                    && vote.IsLike == ((int)VoteType.Neutral))
                {
                    Logger.LogError($"Try to unlike image that is not mark as like. VoteId:{vote.Id}");
                    return false;
                }

                vote.IsLike = isLike ? (int)VoteType.Like : (int)VoteType.Neutral;
            }
            else
            {
                if (!isLike)
                {
                    Logger.LogError($"Try to unlike image that is not mark as like.");
                    return false;
                }

                vote = new Vote
                {
                    ImageId = imageId,
                    AuthorId = userId,
                    IsLike = isLike ? (int)VoteType.Like : (int)VoteType.Neutral,
                };

                await VotesRepository.AddAsync(vote);
            }

            int result = await VotesRepository.SaveChangesAsync();
            return result == 1;
        }
    }
}
