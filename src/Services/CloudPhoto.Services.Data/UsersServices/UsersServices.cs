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

        public T GetUserInfo<T>(string infoForUserId, string currentLoginUser)
        {
            var parameters = new
            {
                infoForUserId,
                currentLoginUser,
                ClaimType = GlobalConstants.ExternalClaimAvatar,
            };

            StringBuilder sqlSelect = new StringBuilder();

            // add head select
            sqlSelect.Append(
                @"SELECT aspu.*, 
                c.ClaimValue AS UserAvatar,
                -- get follow info
                (SELECT Count(*) FROM UserSubscribes AS us
                WHERE us.UserSubscribedId = @currentLoginUser
                AND aspu.Id = us.SubscribeToUserId) AS IsFollowCurrentUser,
                (SELECT Count(*) FROM UserSubscribes AS us
                WHERE aspu.Id = us.SubscribeToUserId) AS CountFollowers,
                (SELECT Count(*) FROM UserSubscribes AS us
                WHERE aspu.Id = us.UserSubscribedId) AS CountFollowing
                FROM AspNetUsers AS aspu
                LEFT JOIN AspNetUserClaims AS c On aspu.Id = c.UserId AND c.ClaimType = @ClaimType
                WHERE aspu.Id = @InfoForUserId");

            return this.DapperService.GetAll<T>(sqlSelect.ToString(), parameters, commandType: CommandType.Text).FirstOrDefault();
        }
    }
}
