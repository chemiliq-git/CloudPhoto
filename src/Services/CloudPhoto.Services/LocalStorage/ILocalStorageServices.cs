namespace CloudPhoto.Services.LocalStorage
{
    using System.Threading.Tasks;

    using CloudPhoto.Services.RemoteStorage;

    public interface ILocalStorageServices
    {
        public Task<StoreFileInfo> UploadFile(UploadDataInfo uploadInfo);

        public Task<bool> DeleteData(StoreFileInfo fileInfo);
    }
}
