namespace CloudPhoto.Web.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using CloudPhoto.Services.ImageValidate;
    using CloudPhoto.Services.LocalStorage;
    using CloudPhoto.Services.RemoteStorage;
    using CloudPhoto.Web.ViewModels.Files;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    public class UploadFilesController : Controller
    {
        public UploadFilesController(
            ILocalStorageServices localStorageService,
            IRemoteStorageService remoteStorageService,
            IImageValidatorService imageValidator,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            this.LocalStorageService = localStorageService;
            this.RemoteStorageService = remoteStorageService;
            this.ImageValidator = imageValidator;
            this.Env = env;
            this.Configuration = configuration;
        }

        public ILocalStorageServices LocalStorageService { get; }

        public IRemoteStorageService RemoteStorageService { get; }

        public IImageValidatorService ImageValidator { get; }

        public IWebHostEnvironment Env { get; }

        public IConfiguration Configuration { get; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(IFormFile file)
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
                    return this.Json(new ResponseUploadFileController() { Result = false, ErrorMessage = "Not valid image format" });
                }

                StoreFileInfo info = await this.UploadFileToLocalFolder(file);
                if (info.BoolResult)
                {
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

                //StoreFileInfo info = await this.RemoteStorageService.UploadFile(new UploadDataInfo(file, "WebPictures", string.Empty));
                //if (info.BoolResult)
                //{
                //    return this.Json(new ResponseUploadFileController() { Result = true, ImageUrl = info.FileAddress });
                //}
                //else
                //{
                //    return this.Json(new ResponseUploadFileController() { Result = false, ErrorMessage = "Error uploading file to storage" });
                //}
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
    }
}
