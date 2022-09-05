namespace CloudPhoto.Services.Data.UsersServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using Mapping;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class UsersService : IUsersService
    {
        public UsersService(
            ILogger<UsersService> logger,
            UserManager<ApplicationUser> userManager,
            IRepository<ApplicationUser> userRepository,
            IRepository<UserSubscribe> userSubscribeRepository)
        {
            Logger = logger;
            UserManager = userManager;
            UserRepository = userRepository;
            UserSubscribeRepository = userSubscribeRepository;
        }

        public ILogger<UsersService> Logger { get; }

        public UserManager<ApplicationUser> UserManager { get; }

        public IRepository<ApplicationUser> UserRepository { get; }

        public IRepository<UserSubscribe> UserSubscribeRepository { get; }

        public async Task<bool> ChangeAvatar(string userId, string avatarUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(avatarUrl))
                {
                    return false;
                }

                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.UserAvatarUrl = avatarUrl;
                IdentityResult result = await UserManager.UpdateAsync(user);

                return result.Succeeded;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error update user avatar for UseId:{userId}");
                return false;
            }
        }

        public T GetUserInfo<T>(string infoForUserId, string currentLoginUserId)
        {
            return GetUsersData<T>(infoForUserId, currentLoginUserId).FirstOrDefault();
        }

        public IEnumerable<T> GetFollowingUsers<T>(string infoForUserId, string currentLoginUserId, int perPage, int page)
        {
            return GetUsersData<T>(infoForUserId, currentLoginUserId, perPage, page, isGetFollowing: true);
        }

        public IEnumerable<T> GetFollowerUsers<T>(string infoForUserId, string currentLoginUserId, int perPage, int page)
        {
            return GetUsersData<T>(infoForUserId, currentLoginUserId, perPage, page, isGetFollower: true);
        }

        private IEnumerable<T> GetUsersData<T>(
            string infoForUserId,
            string currentLoginUserId,
            int perPage = 0,
            int page = 0,
            bool isGetFollower = false,
            bool isGetFollowing = false)
        {
            var selectUserInfo = from user in UserRepository.All()
                                     let isFollowCurrentUser = (from subscribe in UserSubscribeRepository.All()
                                                                where subscribe.UserSubscribedId == currentLoginUserId
                                                                && user.Id == subscribe.SubscribeToUserId
                                                                select subscribe).Count()
                                     let countFollowers = (from subscribe in UserSubscribeRepository.All()
                                                           where user.Id == subscribe.SubscribeToUserId
                                                           select subscribe).Count()
                                     let countFollowing = (from subscribe in UserSubscribeRepository.All()
                                                           where user.Id == subscribe.UserSubscribedId
                                                           select subscribe).Count()
                                     select new ApplicationUser()
                                     {
                                         Id = user.Id,
                                         FirstName = user.FirstName,
                                         LastName = user.LastName,
                                         PayPalEmail = user.PayPalEmail,
                                         Description = user.Description,
                                         UserAvatarUrl = user.UserAvatarUrl,
                                         IsFollowCurrentUser = isFollowCurrentUser > 0,
                                         CountFollowers = countFollowers,
                                         CountFollowing = countFollowing,
                                     };

            if (!isGetFollower
                && !isGetFollowing)
            {
                selectUserInfo = selectUserInfo.Where(x => x.Id == infoForUserId);
            }
            else
            {
                if (isGetFollower)
                {
                    selectUserInfo = selectUserInfo.Where(
                        temp => UserSubscribeRepository.All().
                        Where(x => x.SubscribeToUserId == infoForUserId).Select(x => x.UserSubscribedId).Contains(temp.Id));
                }

                if (isGetFollowing)
                {
                    selectUserInfo = selectUserInfo.Where(
                     temp => UserSubscribeRepository.All().
                     Where(x => x.UserSubscribedId == infoForUserId).Select(x => x.SubscribeToUserId).Contains(temp.Id));
                }
            }

            if (page > 0 &&
                perPage > 0)
            {
                selectUserInfo = selectUserInfo.Skip((page - 1) * perPage).Take(perPage);
            }

            return selectUserInfo.To<T>();
        }
    }
}
