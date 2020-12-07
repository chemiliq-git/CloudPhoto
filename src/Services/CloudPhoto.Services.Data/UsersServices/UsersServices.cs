namespace CloudPhoto.Services.Data.UsersServices
{
    using System.Threading.Tasks;

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

        public T GetUserInfo<T>(string userId)
        {
            throw new System.NotImplementedException();
        }
    }
}
