namespace CloudPhoto.Services.Data.ImagiesService
{
    using CloudPhoto.Data.Models;

    public class ResponseSearchImageModelData
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ThumbnailImageUrl { get; set; }

        public string ImageUrl { get; set; }

        public string ImageType { get; set; }

        public string AuthorId { get; set; }

        public string AuthorAvatarUrl { get; set; }

        public bool IsFollow { get; set; }

        public bool IsLike { get; set; }

        public int LikeCount { get; set; }
    }
}
