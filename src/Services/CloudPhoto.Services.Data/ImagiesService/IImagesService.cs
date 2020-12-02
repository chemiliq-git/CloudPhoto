namespace CloudPhoto.Services.Data.ImagiesService
{
    using CloudPhoto.Common;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IImagesService
    {
        public Task<string> CreateAsync(CreateImageModelData createData);

        public Task<bool> UpdateAsync(string id, string name, string description);

        public Task<bool> Delete(string id);

        public T GetByCategoryId<T>(string categoryId);

        public IEnumerable<T> GetByFilter<T>(
            SearchImageData searchData,
            int page = 1,
            int perPage = GlobalConstants.ImagePerPageDefaultValue);
    }
}
