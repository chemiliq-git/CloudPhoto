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
    using Microsoft.AspNetCore.Identity;

    public class UsersServices : IUsersServices
    {
        public UsersServices(
            UserManager<ApplicationUser> userManager,
            IRepository<ApplicationUser> userRepository,
            IDapperService dapperService)
        {
            this.UserManager = userManager;
            this.UserRepository = userRepository;
            this.DapperService = dapperService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public IRepository<ApplicationUser> UserRepository { get; }

        public IDapperService DapperService { get; }

        public async Task<bool> ChangeAvatar(string userId, string avatarId)
        {
            try
            {
                ApplicationUser user = await this.UserManager.FindByIdAsync(userId);
                IList<Claim> lstClaims = await this.UserManager.GetClaimsAsync(user);
                Claim existAvatar = lstClaims.FirstOrDefault(temp => temp.Type == GlobalConstants.ExternalClaimAvatar);

                IdentityResult result = null;
                if (existAvatar != null)
                {
                    result = await this.UserManager.RemoveClaimAsync(user, existAvatar);
                }

                if (result == null
                    || result.Succeeded)
                {
                    user.Claims.Add(new IdentityUserClaim<string>()
                    {
                        ClaimType = GlobalConstants.ExternalClaimAvatar,
                        ClaimValue = avatarId,
                        UserId = user.Id,
                    });

                    result = await this.UserManager.UpdateAsync(user);
                }

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
            var parameters = new
            {
                Skip = (page - 1) * perPage,
                Take = perPage,
                infoForUserId,
                currentLoginUserId,
                ClaimType = GlobalConstants.ExternalClaimAvatar,
            };

            StringBuilder sqlSelect = new StringBuilder();

            // add head select
            sqlSelect.Append(
                @"SELECT aspu.*, 
                c.ClaimValue AS UserAvatar,
                -- get follow info
                (SELECT Count(*) FROM UserSubscribes AS us
                WHERE us.UserSubscribedId = @currentLoginUserId
                AND aspu.Id = us.SubscribeToUserId) AS IsFollowCurrentUser,
                -- get count followers
                (SELECT Count(*) FROM UserSubscribes AS us
                WHERE aspu.Id = us.SubscribeToUserId) AS CountFollowers,
                -- get count following
                (SELECT Count(*) FROM UserSubscribes AS us
                WHERE aspu.Id = us.UserSubscribedId) AS CountFollowing
                FROM AspNetUsers AS aspu
                LEFT JOIN AspNetUserClaims AS c On aspu.Id = c.UserId AND c.ClaimType = @ClaimType");

            if (!isGetFollower
                && !isGetFollowing)
            {
                sqlSelect.Append(" WHERE aspu.Id = @InfoForUserId");
            }
            else
            {
                if (isGetFollower)
                {
                    sqlSelect.Append(@" WHERE aspu.id in 
                    (SELECT UserSubscribedId FROM UserSubscribes WHERE SubscribeToUserId = @InfoForUserId)");
                }

                if (isGetFollowing)
                {
                    sqlSelect.Append(@" WHERE aspu.id in 
                    (SELECT SubscribeToUserId FROM UserSubscribes WHERE UserSubscribedId = @InfoForUserId)");
                }
            }

            if (page > 0 &&
                perPage > 0)
            {
                sqlSelect.AppendLine(" ORDER BY aspu.ID OFFSET @Skip ROWS ");
                sqlSelect.AppendLine(" FETCH NEXT @Take ROWS ONLY");
            }

            return this.DapperService.GetAll<T>(sqlSelect.ToString(), parameters, commandType: CommandType.Text);
        }
    }
}
