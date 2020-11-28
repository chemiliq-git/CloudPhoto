using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Images
{
    public class CreateImageViewModel : IMapFrom<Image>
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        [Required]
        [Display(Name = "Tags")]
        public string ImageTags { get; set; }

        public IEnumerable<CategoryDropDownViewModel> Categories { get; set; }
    }
}