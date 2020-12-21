using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Home
{
    public class ImageHomeViewModel : IMapFrom<Image>
    {
        public string Id { get; set; }

        public string ThumbnailImageUrl { get; set; }
    }
}
