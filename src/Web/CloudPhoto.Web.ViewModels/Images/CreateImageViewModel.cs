using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;
using CloudPhoto.Web.ViewModels.Categories;
using Newtonsoft.Json;

namespace CloudPhoto.Web.ViewModels.Images
{
    public class CreateImageViewModel : IMapFrom<Image>, IValidatableObject
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Required]
        public string ImageId { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        [Required]
        [Display(Name = "Tags")]
        public string ImageTags { get; set; }

        public IEnumerable<CategoryDropDownViewModel> Categories { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ValidationResult error = null;
            try
            {
                List<string> parse = JsonConvert.DeserializeObject<List<string>>(this.ImageTags);
                if (parse == null
                    || parse.Count == 0)
                {
                    error = new ValidationResult(
                        $"Can not parse image tag.",
                        new[] { nameof(this.ImageTags) });
                }
            }
            catch (Exception)
            {
                error = new ValidationResult(
                    $"Can not parse image tag.",
                    new[] { nameof(this.ImageTags) });
            }

            if (error != null)
            {
                yield return error;
            }
        }
    }
}