namespace CloudPhoto.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using CloudPhoto.Data.Common.Models;

    public class Category : BaseDeletableModel<string>, ISortOrderMode
    {
        public Category()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public virtual ICollection<ImageCategory> ImageCategories { get; set; }
    }
}
