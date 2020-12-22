namespace CloudPhoto.Services.Data.ImagiesService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IImagesService
    {
        public Task<string> CreateAsync(CreateImageModelData createData);

        public T GetImageById<T>(string imageId);

        public IEnumerable<T> GetByFilter<T>(
            SearchImageData searchData,
            int perPage,
            int page = 1);

        public IEnumerable<T> GetMostLikeImageByCategory<T>(string categoryId, int countTopImage);
    }
}
