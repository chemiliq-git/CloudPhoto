namespace CloudPhoto.Services.Data.VotesService
{
    using System.Threading.Tasks;

    public interface IVotesService
    {
        Task<bool> VoteAsync(string imageId, string userId, bool isLike);
    }
}
