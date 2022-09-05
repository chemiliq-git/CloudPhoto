namespace CloudPhoto.Services.Data.SubscribesService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using Mapping;
    using Microsoft.Extensions.Logging;

    public class SubscribesService : ISubscribesService
    {
        public SubscribesService(
            ILogger<SubscribesService> logger,
            IDeletableEntityRepository<UserSubscribe> subsctibeRepository)
        {
            Logger = logger;
            SubsctibeRepository = subsctibeRepository;
        }

        public ILogger<SubscribesService> Logger { get; }

        public IDeletableEntityRepository<UserSubscribe> SubsctibeRepository { get; }

        public IEnumerable<T> GetSubscribes<T>(string userSubscribedId = null, string subscribeToUserId = null)
        {
            IQueryable<UserSubscribe> query =
                           SubsctibeRepository.All();

            if (!string.IsNullOrEmpty(userSubscribedId))
            {
                query = query.Where(temp => temp.UserSubscribedId == userSubscribedId);
            }

            if (!string.IsNullOrEmpty(subscribeToUserId))
            {
                query = query.Where(temp => temp.SubscribeToUserId == subscribeToUserId);
            }

            return query.To<T>();
        }

        public async Task<bool> ManageUserSubsctibe(string userSubscribedId, string subscribeToUserId, bool isWantToSubscribe)
        {
            if (string.Compare(userSubscribedId, subscribeToUserId, true) == 0)
            {
                return false;
            }

            UserSubscribe existSubscribe =
               GetSubscribes<UserSubscribe>(userSubscribedId, subscribeToUserId).FirstOrDefault();

            if (isWantToSubscribe)
            {
                if (existSubscribe != null)
                {
                    return false;
                }

                string newId = await CreateAsync(userSubscribedId, subscribeToUserId);
                return !string.IsNullOrEmpty(newId);
            }
            else
            {
                if (existSubscribe == null)
                {
                    return false;
                }

                return await Delete(existSubscribe);
            }
        }

        private async Task<string> CreateAsync(string userSubscribedId, string subscribeToUserId)
        {
            var subscribeRow = new UserSubscribe
            {
                UserSubscribedId = userSubscribedId,
                SubscribeToUserId = subscribeToUserId,
            };

            await SubsctibeRepository.AddAsync(subscribeRow);

            await SubsctibeRepository.SaveChangesAsync();

            return subscribeRow.Id;
        }

        private async Task<bool> Delete(UserSubscribe record)
        {
            SubsctibeRepository.HardDelete(record);
            int result = await SubsctibeRepository.SaveChangesAsync();
            return result == 1;
        }
    }
}
