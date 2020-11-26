namespace CloudPhoto.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using CloudPhoto.Data.Common.Models;

    public class Image : BaseDeletableModel<string>
    {
        public Image()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string ImageType { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
