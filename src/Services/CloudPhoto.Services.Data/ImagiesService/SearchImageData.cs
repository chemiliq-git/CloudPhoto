namespace CloudPhoto.Services.Data.ImagiesService
{
    using System.Collections.Generic;

    public class SearchImageData
    {
        public SearchImageData()
        {
            this.FilterCategory = new List<string>();
            this.FilterTags = new List<string>();
        }

        public List<string> FilterCategory { get; set; }

        public List<string> FilterTags { get; set; }

        public string FilterByTag { get; set; }

        public string AuthorId { get; set; }
    }
}
