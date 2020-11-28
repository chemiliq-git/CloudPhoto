namespace CloudPhoto.Services.RemoteStorage
{
    using System.Threading.Tasks;

    public interface IRemoteStorageService
    {
        public Task<StoreFileInfo> UploadFile(UploadDataInfo uploadInfo);

        public Task<bool> DeleteData(StoreFileInfo fileInfo);
    }
}
