using System.Linq;

using AutoMapper;
using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Images
{
    public class ListImageViewModel : IMapFrom<Image>
    {
        public string Id { get; set; }

        public int ImageIndex { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ThumbnailImageUrl { get; set; }

        public bool IsLike { get; set; }

        public string AuthorId { get; set; }

        public string AuthorAvatarUrl { get; set; }
    }
}
