using System.Linq;

using AutoMapper;
using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Images
{
    public class ImagePreviewViewModel : IMapFrom<Image>
    {
        public string Id { get; set; }

        public int ImageIndex { get; set; }

        public bool IsEndedImage { get; set; } = false;

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool IsLike { get; set; } = false;

        public int LikeCount { get; set; } = 0;

        public string AuthorUserName { get; set; }
    }
}
