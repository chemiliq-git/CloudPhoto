namespace CloudPhoto.Data.Models
{
    using System;

    using CloudPhoto.Data.Common.Models;

    public class ImageCategory : BaseDeletableModel<string>
    {
        public ImageCategory()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string ImageId { get; set; }

        public virtual Image Image { get; set; }

        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
