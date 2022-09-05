namespace CloudPhoto.Services.RemoteStorage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class CloudinaryStorageServer : IRemoteStorageService
    {
        /*
        appsetting.json section
        "Cloudinary": {
            "CloudName": "your cloudinary name",
            "APIKey": "your api",
            "APISecret": "your api secret"
          },
         */

        private readonly string cloudName = string.Empty;
        private readonly string apiKey = string.Empty;
        private readonly string apiSecret = string.Empty;

        public CloudinaryStorageServer(
            IConfiguration configuration,
            ILogger<CloudinaryStorageServer> logger)
        {
            Configuration = configuration;
            Logger = logger;

            cloudName = Configuration.GetSection("Cloudinary:CloudName")?.Value;
            apiKey = Configuration.GetSection("Cloudinary:APIKey")?.Value;
            apiSecret = Configuration.GetSection("Cloudinary:APISecret")?.Value;

            if (!ValidaAccountInf())
            {
                throw new Exception("Cloudinary provider is not initialize correctly!!!");
            }
        }

        public IConfiguration Configuration { get; }

        public ILogger<CloudinaryStorageServer> Logger { get; }

        public async Task<bool> DeleteData(StoreFileInfo fileInfo)
        {
            try
            {
                Account account = new Account(
                       cloudName,
                       apiKey,
                       apiSecret);

                Cloudinary cloudinary = new Cloudinary(account);
                DeletionParams deletionParams = new DeletionParams(fileInfo.FileId);
                DeletionResult result = await cloudinary.DestroyAsync(deletionParams);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<StoreFileInfo> UploadFile(UploadDataInfo uploadInfo)
        {
            try
            {
                if (uploadInfo.Stream == null
                   || uploadInfo.Stream.Length == 0
                   || string.IsNullOrEmpty(uploadInfo.FileName))
                {
                    Logger.LogError("File is required");
                    return new StoreFileInfo(false);
                }

                Account account = new Account(
                    cloudName,
                    apiKey,
                    apiSecret);

                Cloudinary cloudinary = new Cloudinary(account);

                var uploadParams = new ImageUploadParams();

                ImageUploadResult uploadResult;
                using (MemoryStream stream = new MemoryStream())
                {
                    uploadInfo.Stream.CopyTo(stream);
                    stream.Position = 0;
                    uploadParams.File = new FileDescription(uploadInfo.FileName, stream);
                    if (!string.IsNullOrEmpty(uploadInfo.DirectoryName))
                    {
                        uploadParams.Folder = uploadInfo.DirectoryName;
                    }

                    uploadResult = await cloudinary.UploadAsync(uploadParams);
                }

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return new StoreFileInfo(uploadResult.Uri.AbsoluteUri, uploadResult.PublicId);
                }
                else
                {
                    return new StoreFileInfo(false);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                return new StoreFileInfo(false);
            }
        }

        private bool ValidaAccountInf()
        {
            if (string.IsNullOrEmpty(cloudName))
            {
                Logger.LogError("Cloudinary account is required");
                return false;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                Logger.LogError("Cloudinary apiKey is required");
                return false;
            }

            if (string.IsNullOrEmpty(apiSecret))
            {
                Logger.LogError("Cloudinary apiSecret is required");
                return false;
            }

            return true;
        }
    }
}
