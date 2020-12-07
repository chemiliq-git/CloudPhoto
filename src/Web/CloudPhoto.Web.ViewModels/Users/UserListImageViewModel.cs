using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Users
{
    public class UserListImageViewModel : IMapFrom<Image>
    {
        public string Id { get; set; }

        public int ImageIndex { get; set; }

        public string ThumbnailImageUrl { get; set; }

        public bool IsLike { get; set; }
    }
}
