namespace CloudPhoto.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using CloudPhoto.Data.Common.Models;

    public class Vote : BaseModel<string>
    {
        public Vote()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string ImageId { get; set; }

        public virtual Image Image { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public int IsLike { get; set; }
    }
}
