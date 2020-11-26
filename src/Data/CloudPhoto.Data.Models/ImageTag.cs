namespace CloudPhoto.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using CloudPhoto.Data.Common.Models;

    public class ImageTag : BaseDeletableModel<string>
    {
        public ImageTag()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string ImageId { get; set; }

        public virtual Image Image { get; set; }

        [Required]
        public string TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
