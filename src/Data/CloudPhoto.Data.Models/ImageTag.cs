namespace CloudPhoto.Data.Models
{
    using System;

    using CloudPhoto.Data.Common.Models;

    public class ImageTag : BaseDeletableModel<string>
    {
        public ImageTag()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string ImageId { get; set; }

        public virtual Image Image { get; set; }

        public string TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
