using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Tags
{
    public class SearchTagDataModel : IMapFrom<Tag>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
