namespace CloudPhoto.Services.Data.UsersServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.DapperService;
    using CloudPhoto.Services.Mapping;
    using Microsoft.AspNetCore.Identity;

    public class UsersServices : IUsersServices
    {
        public UsersServices(
            UserManager<ApplicationUser> userManager,
            IRepository<ApplicationUser> userRepository,
            IRepository<UserSubscribe> userSubscribeRepository,
            IDapperService dapperService)
        {
            this.UserManager = userManager;
            this.UserRepository = userRepository;
            this.UserSubscribeRepository = userSubscribeRepository;
            this.DapperService = dapperService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public IRepository<ApplicationUser> UserRepository { get; }

        public IRepository<UserSubscribe> UserSubscribeRepository { get; }

        public IDapperService DapperService { get; }

        public async Task<bool> ChangeAvatar(string userId, string avatarUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(avatarUrl))
                {
                    return false;
                }

                ApplicationUser user = await this.UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.UserAvatarUrl = avatarUrl;
                IdentityResult result = await this.UserManager.UpdateAsync(user);

                return result.Succeeded;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public T GetUserInfo<T>(string infoForUserId, string currentLoginUserId)
        {
            return this.GetUsersData<T>(infoForUserId, currentLoginUserId).FirstOrDefault();
        }

        public IEnumerable<T> GetFollowingUsers<T>(string infoForUserId, string currentLoginUserId, int perPage, int page)
        {
            return this.GetUsersData<T>(infoForUserId, currentLoginUserId, perPage, page, isGetFollowing: true);
        }

        public IEnumerable<T> GetFollowerUsers<T>(string infoForUserId, string currentLoginUserId, int perPage, int page)
        {
            return this.GetUsersData<T>(infoForUserId, currentLoginUserId, perPage, page, isGetFollower: true);
        }

        private IEnumerable<T> GetUsersData<T>(
            string infoForUserId,
            string currentLoginUserId,
            int perPage = 0,
            int page = 0,
            bool isGetFollower = false,
            bool isGetFollowing = false)
        {
            var selectUserInfo = from user in this.UserRepository.All()
                                     let isFollowCurrentUser = (from subscribe in this.UserSubscribeRepository.All()
                                                                where subscribe.UserSubscribedId == currentLoginUserId
                                                                && user.Id == subscribe.SubscribeToUserId
                                                                select subscribe).Count()
                                     let countFollowers = (from subscribe in this.UserSubscribeRepository.All()
                                                           where user.Id == subscribe.SubscribeToUserId
                                                           select subscribe).Count()
                                     let countFollowing = (from subscribe in this.UserSubscribeRepository.All()
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
                        temp => this.UserSubscribeRepository.All().
                        Where(x => x.SubscribeToUserId == infoForUserId).Select(x => x.UserSubscribedId).Contains(temp.Id));
                }

                if (isGetFollowing)
                {
                    selectUserInfo = selectUserInfo.Where(
                     temp => this.UserSubscribeRepository.All().
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
