namespace CloudPhoto.Web.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Data.Models;
    using CloudPhoto.Services.Data.BackgroundServices;
    using CloudPhoto.Services.Data.BackgroundServices.BackgroundQueue;
    using Services.ImageValidate;
    using Services.LocalStorage;
    using Services.RemoteStorage;
    using ViewModels.Files;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    public class UploadFilesController : BaseController
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
            LocalStorageService = localStorageService;
            RemoteStorageService = remoteStorageService;
            ImageValidator = imageValidator;
            UserManager = userManager;
            Env = env;
            Configuration = configuration;
            Queue = queue;
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
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    return BadRequest();
                }

                ImageValidateResult result = ImageValidator.ValidateImageFile(file);
                if (!result.IsValid)
                {
                    return Json(new ResponseUploadFileController() { Result = false, ErrorMessage = "Invalid image format!" });
                }

                if (!IsCorrectImageSize(file, out string errMessage))
                {
                    return Json(new ResponseUploadFileController() { Result = false, ErrorMessage = errMessage });
                }

                StoreFileInfo info = await UploadFileToLocalFolder(file);

                if (info.BoolResult)
                {
                    string filePath = Env.WebRootPath + info.FileAddress;

                    // start backgroun process
                    Queue.Enqueue(new ImageInfoParams() { ImageId = info.FileId, ImagePath = filePath });

                    return Json(new ResponseUploadFileController()
                    {
                        Result = true,
                        ImageUrl = info.FileAddress,
                        FileId = info.FileId,
                    });
                }
                else
                {
                    return Json(new ResponseUploadFileController()
                    {
                        Result = false,
                        ErrorMessage = "Error uploading file to storage",
                    });
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("UploadAvatart")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UploadAvatart(IFormFile file, string userId)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    return BadRequest();
                }

                ApplicationUser user = await UserManager.GetUserAsync(User);
                if (user.Id != userId)
                {
                    return BadRequest();
                }

                ImageValidateResult result = ImageValidator.ValidateImageFile(file);
                if (!result.IsValid)
                {
                    return Json(new ResponseUploadFileController() { Result = false, ErrorMessage = "Invalid image format!" });
                }

                if (!IsCorrectAcatarSize(file, out string errorMessage))
                {
                    return Json(new ResponseUploadFileController() { Result = false, ErrorMessage = errorMessage });
                }

                StoreFileInfo info;
                using (MemoryStream memory = new MemoryStream())
                {
                    file.CopyTo(memory);
                    memory.Position = 0;

                    info = await RemoteStorageService.UploadFile(new UploadDataInfo(
                                file.FileName,
                                memory,
                                "WebPictures",
                                string.Empty));
                }

                if (info.BoolResult)
                {
                    return Json(new ResponseUploadFileController() { Result = true, ImageUrl = info.FileAddress });
                }
                else
                {
                    return Json(new ResponseUploadFileController() { Result = false, ErrorMessage = "Error uploading file to storage" });
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<StoreFileInfo> UploadFileToLocalFolder(IFormFile file)
        {
            string fileId = Guid.NewGuid().ToString();
            string folderForResize = Path.Combine(
                Env.WebRootPath,
                Configuration.GetSection("Images:LocalImageFolder").Value,
                fileId);

            using MemoryStream stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;
            StoreFileInfo info = await LocalStorageService.UploadFile(
                new UploadDataInfo(
                    file.FileName,
                    stream,
                    folderForResize));
            info.FileId = fileId;
            info.FileAddress = info.FileAddress.Replace(Env.WebRootPath, string.Empty);
            return info;
        }

        private bool IsCorrectImageSize(IFormFile file, out string errMessage)
        {
            string strSettingSize = Configuration.GetSection("Images:MinimumImageSizeMB")?.Value;
            if (!int.TryParse(strSettingSize, out int intSettingSize))
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
            string strSettingSize = Configuration.GetSection("Images:MaxAvatarSizeKB")?.Value;
            if (!int.TryParse(strSettingSize, out int intSettingSize))
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
