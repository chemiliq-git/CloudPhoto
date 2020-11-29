namespace CloudPhoto.Services.Data.ImagiesService
{
    using System.Collections.Generic;

    public class SearchImageData
    {
        public List<string> FilterCategory { get; set; }

        public List<string> FilterTags { get; set; }

        public string AuthorId { get; set; }
    }
}
