using System.Linq;

using AutoMapper;
using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Images
{
    public class ImagePreviewViewModel : IMapFrom<Image>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public int ImageIndex { get; set; }

        public bool IsEndedImage { get; set; } = false;

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool IsLike { get; set; }

        public string AuthorUserName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Image, ImagePreviewViewModel>()
                .ForMember(x => x.IsLike, options =>
                {
                    options.MapFrom(p => p.Votes.Sum(v => (int)v.IsLike) == 1);
                });
        }
    }
}
