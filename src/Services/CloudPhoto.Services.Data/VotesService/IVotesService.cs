namespace CloudPhoto.Services.Data.VotesService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IVotesService
    {
        Task<bool> VoteAsync(string imageId, string userId, bool isLike);
    }
}
