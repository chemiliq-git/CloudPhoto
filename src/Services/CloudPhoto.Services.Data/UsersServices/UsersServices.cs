namespace CloudPhoto.Services.Data.UsersServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using CloudPhoto.Data.Models;
    using Microsoft.AspNetCore.Identity;

    public class UsersServices : IUsersServices
    {
        public UsersServices(
            UserManager<ApplicationUser> userManager)
        {
            this.UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; }

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
    }
}
