namespace CloudPhoto.Services.Data.SubscribesService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISubscribesService
    {
        public IEnumerable<T> GetSubscribesAsync<T>(string userSubscribedId = null, string subscribeToUserId = null);

        public Task<bool> ManageUserSubsctibe(string userSubscribedId, string subscribeToUserId, bool isWantToSubscribe);
    }
}
