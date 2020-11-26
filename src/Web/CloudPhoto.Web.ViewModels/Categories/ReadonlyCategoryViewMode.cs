using System;

using CloudPhoto.Data.Models;
using CloudPhoto.Services.Mapping;

namespace CloudPhoto.Web.ViewModels.Categories
{
    public class ReadonlyCategoryViewMode : IMapFrom<Category>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
