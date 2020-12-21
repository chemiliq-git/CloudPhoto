namespace CloudPhoto.Services.Data.ImagiesService
{
    using CloudPhoto.Data.Models;

    public class ImageLikeData
    {
        public Image Image { get; set; }

        public int LikeCounts { get; set; }
    }
}
