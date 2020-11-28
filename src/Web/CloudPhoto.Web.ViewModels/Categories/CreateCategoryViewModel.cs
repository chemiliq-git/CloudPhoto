using System.ComponentModel.DataAnnotations;

using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Categories
{
    public class CreateCategoryViewModel : IMapFrom<Category>
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
