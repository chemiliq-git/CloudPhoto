using System.Collections.Generic;

using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;
using CloudPhoto.Web.ViewModels.Images;

namespace CloudPhoto.Web.ViewModels.Users
{
    public class UserPreviewViewModel : IMapFrom<ApplicationUser>
    {
        public UserPreviewViewModel()
        {
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserAvatar { get; set; }
    }
}
