namespace CloudPhoto.Web.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.BackgroundServices;
    using CloudPhoto.Services.Data.BackgroundServices.BackgroundQueue;
    using CloudPhoto.Services.ImageValidate;
    using CloudPhoto.Services.LocalStorage;
    using CloudPhoto.Services.RemoteStorage;
    using CloudPhoto.Web.ViewModels.Files;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    public class UploadFilesController : Controller
    {
        public UploadFilesController(
            ILocalStorageServices localStorageService,
            IRemoteStorageService remoteStorageService,
            IImageValidatorService imageValidator,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IConfiguration configuration,
            IBackgroundQueue<ImageInfoParams> queue)
        {
            this.LocalStorageService = localStorageService;
            this.RemoteStorageService = remoteStorageService;
            this.ImageValidator = imageValidator;
            this.UserManager = userManager;
            this.Env = env;
            this.Configuration = configuration;
            this.Queue = queue;
        }

        public ILocalStorageServices LocalStorageService { get; }

        public IRemoteStorageService RemoteStorageService { get; }

        public IImageValidatorService ImageValidator { get; }

        public UserManager<ApplicationUser> UserManager { get; }

        public IWebHostEnvironment Env { get; }

        public IConfiguration Configuration { get; }

        public IBackgroundQueue<ImageInfoParams> Queue { get; }

        [HttpPost("UploadImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (this.ModelState.IsValid)
            {
                if (file == null)
                {
                    return this.BadRequest();
                }

                ImageValidateResult result = this.ImageValidator.ValidateImageFile(file);
                if (!result.IsValid)
                {
                    return this.Json(new ResponseUploadFileController() { Result = false, ErrorMessage = "Invalid image format!" });
                }

                string errMessage;
                if (!this.IsCorrectImageSize(file, out errMessage))
                {
                    return this.Json(new ResponseUploadFileController() { Result = false, ErrorMessage = errMessage });
                }

                StoreFileInfo info = await this.UploadFileToLocalFolder(file);

                if (info.BoolResult)
                {
                    string filePath = this.Env.WebRootPath + info.FileAddress;

                    // start backgroun process
                    this.Queue.Enqueue(new ImageInfoParams() { ImageId = info.FileId, ImagePath = filePath });

                    return this.Json(new ResponseUploadFileController()
                    {
                        Result = true,
                        ImageUrl = info.FileAddress,
                        FileId = info.FileId,
                    });
                }
                else
                {
                    return this.Json(new ResponseUploadFileController()
                    {
                        Result = false,
                        ErrorMessage = "Error uploading file to storage",
                    });
                }
            }
            else
            {
                return this.BadRequest();
            }
        }

        [HttpPost("UploadAvatart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatart(IFormFile file, string userId)
        {
            if (this.ModelState.IsValid)
            {
                if (file == null)
                {
                    return this.BadRequest();
                }

                ApplicationUser user = await this.UserManager.GetUserAsync(this.User);
                if (user.Id != userId)
                {
                    return this.BadRequest();
                }

                ImageValidateResult result = this.ImageValidator.ValidateImageFile(file);
                if (!result.IsValid)
                {
                    return this.Json(new ResponseUploadFileController() { Result = false, ErrorMessage = "Invalid image format!" });
                }

                string errorMessage;
                if (!this.IsCorrectAcatarSize(file, out errorMessage))
                {
                    return this.Json(new ResponseUploadFileController() { Result = false, ErrorMessage = errorMessage });
                }

                StoreFileInfo info;
                using (MemoryStream memory = new MemoryStream())
                {
                    file.CopyTo(memory);
                    memory.Position = 0;

                    info = await this.RemoteStorageService.UploadFile(new UploadDataInfo(
                                file.FileName,
                                memory,
                                "WebPictures",
                                string.Empty));
                }

                if (info.BoolResult)
                {
                    return this.Json(new ResponseUploadFileController() { Result = true, ImageUrl = info.FileAddress });
                }
                else
                {
                    return this.Json(new ResponseUploadFileController() { Result = false, ErrorMessage = "Error uploading file to storage" });
                }
            }
            else
            {
                return this.BadRequest();
            }
        }

        private async Task<StoreFileInfo> UploadFileToLocalFolder(IFormFile file)
        {
            string fileId = Guid.NewGuid().ToString();
            string folderForResize = Path.Combine(
                this.Env.WebRootPath,
                this.Configuration.GetSection("Images:LocalImageFolder").Value,
                fileId);

            using MemoryStream stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;
            StoreFileInfo info = await this.LocalStorageService.UploadFile(
                new UploadDataInfo(
                    file.FileName,
                    stream,
                    folderForResize));
            info.FileId = fileId;
            info.FileAddress = info.FileAddress.Replace(this.Env.WebRootPath, "");
            return info;
        }

        private bool IsCorrectImageSize(IFormFile file, out string errMessage)
        {
            string strSettingSize = this.Configuration.GetSection("Images:MinimumImageSizeMB")?.Value;
            int intSettingSize;
            if (!int.TryParse(strSettingSize, out intSettingSize))
            {
                intSettingSize = 2;
            }

            int minimumSize = intSettingSize * 1048576;

            if (file.Length < minimumSize)
            {
                errMessage = $"Upload file must great by {intSettingSize} MB";
                return false;
            }
            else
            {
                errMessage = string.Empty;
                return true;
            }
        }

        private bool IsCorrectAcatarSize(IFormFile file, out string errMessage)
        {
            string strSettingSize = this.Configuration.GetSection("Images:MaxAvatarSizeKB")?.Value;
            int intSettingSize;
            if (!int.TryParse(strSettingSize, out intSettingSize))
            {
                intSettingSize = 50;
            }

            int maxSize = intSettingSize * 1000;

            if (file.Length >= maxSize)
            {
                errMessage = $"Avatar must be smaller by {intSettingSize} KB";
                return false;
            }
            else
            {
                errMessage = string.Empty;
                return true;
            }
        }
    }
}
