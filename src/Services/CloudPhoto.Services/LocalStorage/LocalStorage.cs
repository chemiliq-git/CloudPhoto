namespace CloudPhoto.Services.LocalStorage
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;

    using RemoteStorage;
    using Microsoft.Extensions.Logging;

    public class LocalStorage : ILocalStorageServices
    {
        public LocalStorage(ILogger<LocalStorage> logger)
        {
            Logger = logger;
        }

        public ILogger<LocalStorage> Logger { get; }

        public Task<bool> DeleteData(StoreFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<StoreFileInfo> UploadFile(UploadDataInfo uploadInfo)
        {
            try
            {
                if (!Directory.Exists(uploadInfo.DirectoryName))
                {
                    Directory.CreateDirectory(uploadInfo.DirectoryName);
                }

                string fileName = Path.Combine(uploadInfo.DirectoryName, uploadInfo.FileName);
                using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await uploadInfo.Stream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }

                return new StoreFileInfo(fileName, string.Empty);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error when save local file");
                return new StoreFileInfo(false);
            }
        }
    }
}
