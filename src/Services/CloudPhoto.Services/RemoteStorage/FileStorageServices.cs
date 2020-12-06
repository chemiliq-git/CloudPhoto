namespace CloudPhoto.Services.RemoteStorage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Storage.Files.Shares;
    using Azure.Storage.Files.Shares.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class FileStorageServices : IRemoteStorageService
    {
        private readonly string connection = string.Empty;

        public FileStorageServices(
            IConfiguration configuration)
        {
            this.connection = configuration.GetSection("BlobAzureSettings:Connection")?.Value;
        }

        public Task<bool> DeleteData(StoreFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<StoreFileInfo> UploadFile(UploadDataInfo uploadInfo)
        {
            // Azure request only lowerCase container name
            string container = uploadInfo.Container.ToLower();

            // Get a reference to a share and then create it
            ShareClient share = new ShareClient(this.connection, container);
            share.CreateIfNotExists();

            // Get a reference to a directory and create it
            ShareDirectoryClient directory = share.GetDirectoryClient(uploadInfo.DirectoryName);
            await directory.CreateIfNotExistsAsync();

            // Get a reference to a file and upload it
            ShareFileClient file = directory.GetFileClient(this.GenerateFileName(uploadInfo.FileName));
            Response<ShareFileInfo> uploadFile = file.Create(uploadInfo.Stream.Length);
            file.UploadRange(
                new HttpRange(0, uploadInfo.Stream.Length),
                uploadInfo.Stream);

            return new StoreFileInfo(string.Empty, uploadFile.Value.SmbProperties.FileId);
        }

        private string GenerateFileName(string fileName)
        {
            string[] strName = fileName.Split('.');
            string strFileName = Guid.NewGuid().ToString() + "." + strName[^1];
            return strFileName;
        }
    }
}
