namespace CloudPhoto.Services.Data.ImagiesService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CloudPhoto.Common;

    public interface IImagesService
    {
        public Task<string> CreateAsync(string rootFolder, CreateImageModelData createData);

        public Task<bool> Delete(string id);

        public IEnumerable<T> GetByFilter<T>(
            SearchImageData searchData,
            int perPage,
            int page = 1);

        public IEnumerable<T> GetMostLikeImageByCategory<T>(string categoryId, int countTopImage);
    }
}
