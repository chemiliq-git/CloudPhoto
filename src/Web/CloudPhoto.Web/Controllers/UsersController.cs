namespace CloudPhoto.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text.Json;
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.UsersServices;
    using CloudPhoto.Services.Data.VotesService;
    using CloudPhoto.Web.ViewModels.Images;
    using CloudPhoto.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("[controller]")]
    public class UsersController : Controller
    {
        public UsersController(
            UserManager<ApplicationUser> userManager,
            IUsersServices usersServices,
            IImagesService imagesService,
            IVotesService votesService)
        {
            this.UserManager = userManager;
            this.UsersServices = usersServices;
            this.ImagesService = imagesService;
            this.VotesService = votesService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public IUsersServices UsersServices { get; }

        public IImagesService ImagesService { get; }

        public IVotesService VotesService { get; }

        /// <summary>
        /// Generate user info view.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>User info view.</returns>
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.BadRequest();
            }

            ApplicationUser user = await this.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return this.BadRequest();
            }

            IList<Claim> lstClaims = await this.UserManager.GetClaimsAsync(user);

            UserPreviewViewModel model = new UserPreviewViewModel
            {
                Id = user.Id,
                UserName = user.FullName,
                UserAvatar = lstClaims?.FirstOrDefault(temp => temp.Type == GlobalConstants.ExternalClaimAvatar)?.Value,
            };

            return this.View(model);
        }

        [HttpPost("GetPagingData")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPagingData(
            int pageIndex,
            int pageSize,
            string userId,
            string type)
        {
            ApplicationUser user = await this.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return this.BadRequest();
            }

            SearchImageData localSearchData = null;
            if (type == "uploads")
            {
                localSearchData = new SearchImageData
                {
                    AuthorId = user.Id,
                };
            }
            else if (type == "likes")
            {
                localSearchData = new SearchImageData
                {
                    LikeByUser = user.Id,
                };
            }

            if (this.User.Identity.IsAuthenticated)
            {
                ApplicationUser loginUser = await this.UserManager.GetUserAsync(this.User);
                localSearchData.LikeForUserId = loginUser.Id;
            }

            var data = this.ImagesService.GetByFilter<ListImageViewModel>(
                    localSearchData, pageSize, pageIndex);

            int indexOfPage = 1;
            foreach (ListImageViewModel model in data)
            {
                model.ImageIndex = ((pageIndex - 1) * pageSize) + indexOfPage;
                indexOfPage++;
            }

            if (!data.Any())
            {
                return this.Json(string.Empty);
            }
            else
            {
                return this.PartialView("_ImageListPartial", data);
            }
        }

        [HttpGet("PreviewImage")]
        public async Task<IActionResult> PreviewImage(int id)
        {
            if (!this.Request.Cookies.TryGetValue("pagingData", out string readPagingDataCookie))
            {
                return this.BadRequest();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            PagingCookieData cookieSearchData = JsonSerializer.Deserialize<PagingCookieData>(readPagingDataCookie, options);

            ApplicationUser userPreviewProfil = await this.UserManager.FindByIdAsync(cookieSearchData.UserId);
            if (userPreviewProfil == null)
            {
                return this.BadRequest();
            }

            SearchImageData localSearchData = null;
            if (cookieSearchData.Type == "uploads")
            {
                localSearchData = new SearchImageData
                {
                    AuthorId = userPreviewProfil.Id,
                };
            }
            else if (cookieSearchData.Type == "likes")
            {
                localSearchData = new SearchImageData
                {
                    LikeByUser = userPreviewProfil.Id,
                };
            }

            if (this.User.Identity.IsAuthenticated)
            {
                ApplicationUser loginUser = await this.UserManager.GetUserAsync(this.User);
                localSearchData.LikeForUserId = loginUser.Id;
            }

            var data = this.ImagesService.GetByFilter<ImagePreviewViewModel>(
                    localSearchData, 1, id);

            if (!data.Any())
            {
                if (id > 1)
                {
                    data = this.ImagesService.GetByFilter<ImagePreviewViewModel>(
                        localSearchData, 1, id - 1);
                    ImagePreviewViewModel previewImage = data.First();
                    previewImage.ImageIndex = id - 1;
                    previewImage.IsEndedImage = true;

                    return this.View(previewImage);
                }

                return this.Json(string.Empty);
            }
            else
            {
                ImagePreviewViewModel previewImage = data.First();
                previewImage.ImageIndex = id;

                return this.View(previewImage);
            }
        }

        [HttpPost("UpdateAvatar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(string userId, string avatarUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            if (string.IsNullOrEmpty(avatarUrl))
            {
                return this.BadRequest();
            }

            ApplicationUser user = await this.UserManager.GetUserAsync(this.User);
            if (user.Id != userId)
            {
                return this.BadRequest();
            }

            return this.Json(await this.UsersServices.ChangeAvatar(userId, avatarUrl));
        }
    }
}
