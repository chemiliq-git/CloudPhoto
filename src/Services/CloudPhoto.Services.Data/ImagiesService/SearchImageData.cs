namespace CloudPhoto.Services.Data.ImagiesService
{
    using System.Collections.Generic;

    public class SearchImageData
    {
        public SearchImageData()
        {
            FilterCategory = new List<string>();
            FilterTags = new List<string>();
        }

        public List<string> FilterCategory { get; set; }

        public List<string> FilterTags { get; set; }

        public string FilterByTag { get; set; }

        public string AuthorId { get; set; }

        public string LikeByUser { get; set; }

        public string LikeForUserId { get; set; }
    }
}
