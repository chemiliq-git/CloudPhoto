namespace CloudPhoto.Web.Controllers
{
    using System.Threading.Tasks;
    using CloudPhoto.Common;
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

                //await this.LocalStorageService.UploadFile(new UploadDataInfo(file, GlobalConstants.LocalUploadFolder, "Dasd"));

                StoreFileInfo info = await this.RemoteStorageService.UploadFile(new UploadDataInfo(file, "WebPictures", string.Empty));
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
    }
}
