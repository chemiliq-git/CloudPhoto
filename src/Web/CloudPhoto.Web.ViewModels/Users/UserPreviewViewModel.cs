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
            this.UploadImages = new List<ListImageViewModel>();
            this.LikeImages = new List<ListImageViewModel>();
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public string UserAvatar { get; set; }

        public IEnumerable<ListImageViewModel> UploadImages { get; set; }

        public IEnumerable<ListImageViewModel> LikeImages { get; set; }
    }
}
