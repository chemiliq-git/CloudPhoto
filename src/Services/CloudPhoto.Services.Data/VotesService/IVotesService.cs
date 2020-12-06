namespace CloudPhoto.Services.Data.VotesService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IVotesService
    {
        Task<bool> VoteAsync(string imageId, string userId, bool isLike);

        IEnumerable<T> GetByUser<T>(string userId, string imageId = null);

        IEnumerable<T> GetByImage<T>(string imageId, string userId = null);
    }
}
