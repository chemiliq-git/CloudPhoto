using System.Linq;

using AutoMapper;
using CloudPhoto.Data.Models;
using CloudPhoto.Services.Data.ImagiesService;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Images
{
    public class ImagePreviewViewModel : IMapFrom<ResponseSearchImageModelData>
    {
        public string Id { get; set; }

        public int ImageIndex { get; set; }

        public bool IsEndedImage { get; set; } = false;

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string ThumbnailImageUrl { get; set; }

        public bool IsFollow { get; set; }

        public bool IsLike { get; set; } = false;

        public int LikeCount { get; set; } = 0;

        public string AuthorId { get; set; }

        public string AuthorAvatarUrl { get; set; }

        public string AuthorEmail { get; set; }

        public string PayPalEmail { get; set; }

        public string AuthorFullName { get; set; }

        public virtual string AuthorName
        {
            get
            {
                return string.IsNullOrEmpty(AuthorFullName) ? AuthorEmail : AuthorFullName;
            }
        }
    }
}
