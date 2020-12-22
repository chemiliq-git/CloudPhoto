namespace CloudPhoto.Services.Data.BackgroundServices.ImageHelper
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.TempCloudImageService;
    using CloudPhoto.Services.ImageManipulationProvider;
    using CloudPhoto.Services.RemoteStorage;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class ImageHelper : IImageHelper
    {
        private readonly ILogger<ImageHelper> logger;
        private readonly IRemoteStorageService storageService;
        private readonly IImageManipulationProvider imageManipulation;
        private readonly IConfiguration configuration;
        private readonly ITempCloudImagesService tempCloudImage;

        public ImageHelper(
            ILogger<ImageHelper> logger,
            IRemoteStorageService storageService,
            IImageManipulationProvider imageManipulation,
            IConfiguration configuration,
            ITempCloudImagesService tempCloudImage)
        {
            this.logger = logger;
            this.storageService = storageService;
            this.imageManipulation = imageManipulation;
            this.configuration = configuration;
            this.tempCloudImage = tempCloudImage;
        }

        public async Task UploadImage(ImageInfoParams imageInfoParam, CancellationToken cancellationToken = default)
        {
            try
            {
                this.logger.LogInformation("Start upload image on cloud ...");

                if (!await this.SaveThumbnailImage(imageInfoParam))
                {
                    this.logger.LogError($"Has error when upload thumbnail file. ImageId:{imageInfoParam.ImageId}");
                    return;
                }

                if (!await this.SaveOriginalImage(imageInfoParam))
                {
                    this.logger.LogError($"Has error when upload original file. ImageId:{imageInfoParam.ImageId}");
                    return;
                }

                this.DeleteLocalFiles(Path.GetDirectoryName(imageInfoParam.ImagePath));

                this.logger.LogInformation($"Images for ImageId:{imageInfoParam.ImageId} has been uploaded!");
            }
            catch (Exception e)
            {
                this.logger.LogError(e, $"Has error when upload files. ImageId:{imageInfoParam.ImageId}");
            }
        }

        private async Task<bool> SaveOriginalImage(ImageInfoParams imageInfoParam)
        {
            StoreFileInfo remoteImage = await this.UploadOriginalFile(imageInfoParam.ImagePath);
            if (!remoteImage.BoolResult)
            {
                return false;
            }

            TempCloudImage newCloudImage = new TempCloudImage()
            {
                ImageId = imageInfoParam.ImageId,
                FileId = remoteImage.FileId,
                ImageUrl = remoteImage.FileAddress,
                ImageType = (int)ImageType.Original,
            };
            await this.tempCloudImage.CreateAsync(newCloudImage);
            return true;
        }

        private async Task<bool> SaveThumbnailImage(ImageInfoParams imageInfoParam)
        {
            StoreFileInfo remoteImage = await this.GenerateThumbnailImage(imageInfoParam.ImagePath);
            if (!remoteImage.BoolResult)
            {
                return false;
            }

            TempCloudImage newCloudImage = new TempCloudImage()
            {
                ImageId = imageInfoParam.ImageId,
                FileId = remoteImage.FileId,
                ImageUrl = remoteImage.FileAddress,
                ImageType = (int)ImageType.Thumbnail,
            };
            await this.tempCloudImage.CreateAsync(newCloudImage);
            return true;
        }

        private async Task<StoreFileInfo> UploadOriginalFile(string fullPath)
        {
            try
            {
                using FileStream str = new FileStream(fullPath, FileMode.Open);
                using MemoryStream memory = new MemoryStream();
                str.CopyTo(memory);
                memory.Position = 0;
                StoreFileInfo storeFile = await this.storageService.UploadFile(
                    new UploadDataInfo(
                        Path.GetFileName(fullPath),
                        memory,
                        "WebPictures",
                        string.Empty));
                return storeFile;
            }
            catch (Exception e)
            {
                this.logger.LogError("Error upload original file", e);
                return new StoreFileInfo(false);
            }
        }

        private async Task<StoreFileInfo> GenerateThumbnailImage(string fullPath)
        {
            byte[] image = this.ResizeImageToThumbnail(fullPath);

            using MemoryStream cropImage = new MemoryStream(image);
            StoreFileInfo storeFile = await this.storageService.UploadFile(
                new UploadDataInfo(
                    Path.GetFileName(fullPath),
                    cropImage,
                    "WebPictures",
                    string.Empty));
            return storeFile;
        }

        private byte[] ResizeImageToThumbnail(string fullPath)
        {
            using FileStream str = new FileStream(fullPath, FileMode.Open);
            using MemoryStream memory = new MemoryStream();
            str.CopyTo(memory);
            memory.Position = 0;
            return this.imageManipulation.Resize(
                memory,
                int.Parse(this.configuration.GetSection("Images:ThumbnailImageWidth").Value),
                int.Parse(this.configuration.GetSection("Images:ThumbnailImageHeight").Value));
        }

        private void DeleteLocalFiles(string fullPath)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(fullPath);
                Directory.Delete(directoryName, true);
            }
            catch (Exception e)
            {
                this.logger.LogError("Error delete local files", e);
            }
        }
    }
}
