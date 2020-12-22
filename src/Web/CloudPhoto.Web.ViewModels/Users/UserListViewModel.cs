using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Users
{
    public class UserListViewModel : IMapFrom<ApplicationUser>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserAvatarUrl { get; set; }

        public string Description { get; set; }

        public bool IsFollowCurrentUser { get; set; } = false;

        public virtual string DisplayUserName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FirstName))
                {
                    return this.FirstName + " " + this.LastName;
                }
                else
                {
                    return this.UserName;
                }
            }
        }
    }
}
