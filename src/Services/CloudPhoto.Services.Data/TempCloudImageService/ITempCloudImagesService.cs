namespace CloudPhoto.Services.Data.TempCloudImageService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Models;

    public interface ITempCloudImagesService
    {
        public IEnumerable<T> GetByImageId<T>(string imageId);

        public Task<string> CreateAsync(TempCloudImage tempCloudImage);
    }
}
