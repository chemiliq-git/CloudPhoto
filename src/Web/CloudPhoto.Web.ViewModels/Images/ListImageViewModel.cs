using System.Linq;

using AutoMapper;
using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Images
{
    public class ListImageViewModel : IMapFrom<Image>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool IsLike { get; set; }

        public string AuthorUserName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Image, ListImageViewModel>()
                .ForMember(x => x.IsLike, options =>
                {
                    options.MapFrom(p => p.Votes.Sum(v => (int)v.IsLike) == 1);
                });
        }
    }
}
