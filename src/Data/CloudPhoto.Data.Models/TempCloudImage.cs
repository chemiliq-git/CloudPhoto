namespace CloudPhoto.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using CloudPhoto.Data.Common.Models;

    public class TempCloudImage : BaseModel<string>
    {
        public TempCloudImage()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string ImageId { get; set; }

        [Required]
        public int ImageType { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public string FileId { get; set; }
    }
}
