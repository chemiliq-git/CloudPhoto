namespace CloudPhoto.Services.Data.ImagiesService
{
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Mapping;

    public class CreateImageModelData : IMapFrom<Image>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string CategoryId { get; set; }

        public string AuthorId { get; set; }

        public string Tags { get; set; }
    }
}
