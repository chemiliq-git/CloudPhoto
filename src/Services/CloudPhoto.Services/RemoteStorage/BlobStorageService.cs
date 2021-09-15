namespace CloudPhoto.Services.RemoteStorage
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class BlobStorageService : IRemoteStorageService
    {
        /*
         appsetting.json section
         "BlobAzureSettings": {
            "Connection": "your connection",
            "RegexBlogPattern": "[\\/]{2}[a-z.]*\\/(?<container>[a-z]*)\\/(?<folder>[0-9a-zA-z-]*)((%2F)|(\\/))(?<filename>[a-z0-9A-Z-]*.[\\w]{3})"
          },
         */
        private const string FILENAME = "filename";
        private const string CONTAINER = "container";
        private const string FOLDER = "folder";

        private readonly string connection = string.Empty;
        private readonly string pattern = string.Empty;

        public BlobStorageService(
            IConfiguration configuration,
            ILogger<BlobStorageService> logger)
        {
            this.Configuration = configuration;
            this.connection = this.Configuration.GetSection("BlobAzureSettings:Connection")?.Value;
            this.pattern = this.Configuration.GetSection("BlobAzureSettings:RegexBlogPattern")?.Value;
            this.Logger = logger;

            if (string.IsNullOrEmpty(this.connection))
            {
                string message = string.Format("{0} request connection to Azure Store", nameof(BlobStorageService));
                this.Logger.LogError(message);
                throw new Exception(message);
            }

            if (string.IsNullOrEmpty(this.pattern))
            {
                string message = string.Format("{0} request patter to parse Azure AbsoluteUrl", nameof(BlobStorageService));
                this.Logger.LogError(message);
                throw new Exception(message);
            }
        }

        public IConfiguration Configuration { get; }

        public ILogger<BlobStorageService> Logger { get; }

        public async Task<StoreFileInfo> UploadFile(UploadDataInfo uploadInfo)
        {
            try
            {
                string container = string.Empty;
                if (!string.IsNullOrEmpty(uploadInfo.Container))
                {
                    container = uploadInfo.Container.ToLower();
                }
                else
                {
                    this.Logger.LogError("Conainer name is required");
                    return new StoreFileInfo(false);
                }

                if (uploadInfo.Stream == null
                    || uploadInfo.Stream.Length == 0
                    || string.IsNullOrEmpty(uploadInfo.FileName))
                {
                    this.Logger.LogError("File is required");
                    return new StoreFileInfo(false);
                }

                // Get a reference to a container named "sample-container" and then create it
                BlobContainerClient blobContainer = new BlobContainerClient(this.connection, container);
                await blobContainer.CreateIfNotExistsAsync();

                // Get a reference to a blob named "sample-file" in a container named "sample-container"
                BlobClient blob = blobContainer.GetBlobClient(
                    this.GenerateFileName(uploadInfo.DirectoryName, uploadInfo.FileName));

                // Upload local file
                using (MemoryStream stream = new MemoryStream())
                {
                    uploadInfo.Stream.CopyTo(stream);
                    stream.Position = 0;
                    Response<BlobContentInfo> response = await blob.UploadAsync(
                        stream,
                        conditions: null);
                }

                return new StoreFileInfo(blob.Uri.AbsoluteUri, blob.Name);
            }
            catch (Exception e)
            {
                this.Logger.LogError(e, "Can not upload file");
                return new StoreFileInfo(false);
            }
        }

        public async Task<bool> DeleteData(StoreFileInfo fileInfo)
        {
            try
            {
                if (!this.ParseBlockName(fileInfo.FileAddress, out string container, out string folder, out string fileName))
                {
                    return false;
                }

                BlobContainerClient blobContainer = new BlobContainerClient(this.connection, container);
                await blobContainer.CreateIfNotExistsAsync();

                // Get a reference to a blob named "sample-file" in a container named "sample-container"
                BlobClient blob = blobContainer.GetBlobClient(folder + "/" + fileName);
                Response response = await blob.DeleteAsync();
                return true;
            }
            catch (Exception e)
            {
                this.Logger.LogError(e, e.Message);
                return false;
            }
        }

        private bool ParseBlockName(string fileUrl, out string container, out string folder, out string fileName)
        {
            container = string.Empty;
            folder = string.Empty;
            fileName = string.Empty;

            if (string.IsNullOrEmpty(fileUrl))
            {
                return false;
            }

            Match match = Regex.Match(fileUrl, this.pattern);

            if (!match.Success)
            {
                return false;
            }

            fileName = match.Groups[FILENAME].ToString();
            container = match.Groups[CONTAINER].ToString();
            folder = match.Groups[FOLDER].ToString();

            return true;
        }

        private string GenerateFileName(string directoryName, string fileName)
        {
            string[] strName = fileName.Split('.');
            string strFileName = !string.IsNullOrEmpty(directoryName) ?
                directoryName + "/" + Guid.NewGuid().ToString() + "." + strName[^1]
                : Guid.NewGuid().ToString() + "." + strName[^1];
            return strFileName;
        }
    }
}
