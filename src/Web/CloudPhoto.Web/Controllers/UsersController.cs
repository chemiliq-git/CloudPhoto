namespace CloudPhoto.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
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
    public class UsersController : BaseController
    {
        public UsersController(
            UserManager<ApplicationUser> userManager,
            IUsersService usersServices,
            IImagesService imagesService,
            IVotesService votesService)
        {
            this.UserManager = userManager;
            this.UsersServices = usersServices;
            this.ImagesService = imagesService;
            this.VotesService = votesService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public IUsersService UsersServices { get; }

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

            ApplicationUser loggedUser = await this.UserManager.GetUserAsync(this.User);

            ApplicationUser previewUser = await this.UserManager.FindByIdAsync(id);
            if (previewUser == null)
            {
                return this.BadRequest();
            }

            UserPreviewViewModel userInfo = this.UsersServices.GetUserInfo<UserPreviewViewModel>(id, loggedUser?.Id);
            return this.View(userInfo);
        }

        [HttpPost("GetLinkedUsers")]
        public async Task<IActionResult> GetLinkedUsers(
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

            bool isSearchFollowing;
            switch (type)
            {
                case "followers":
                    {
                        isSearchFollowing = false;
                        break;
                    }

                case "following":
                    {
                        isSearchFollowing = true;
                        break;
                    }

                default:
                    {
                        return this.BadRequest();
                    }
            }

            string likeForUserId = string.Empty;
            if (this.User.Identity.IsAuthenticated)
            {
                ApplicationUser loginUser = await this.UserManager.GetUserAsync(this.User);
                likeForUserId = loginUser.Id;
            }

            IEnumerable<UserListViewModel> lstUsersInfo;
            if (isSearchFollowing)
            {
                lstUsersInfo = this.UsersServices.GetFollowingUsers<UserListViewModel>(
                          userId,
                          likeForUserId,
                          pageSize,
                          pageIndex);
            }
            else
            {
                lstUsersInfo = this.UsersServices.GetFollowerUsers<UserListViewModel>(
                        userId,
                        likeForUserId,
                        pageSize,
                        pageIndex);
            }

            return this.PartialView("_UserListPartial", lstUsersInfo);
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

        [HttpPost("Donate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donate(string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            if (string.IsNullOrEmpty(id))
            {
                return this.BadRequest();
            }

            ApplicationUser dbUser = await this.UserManager.FindByIdAsync(id);
            if (dbUser == null)
            {
                return this.BadRequest();
            }

            if (string.IsNullOrEmpty(dbUser.PayPalEmail))
            {
                return this.BadRequest();
            }

            string donateUrl = GenerateDonateUrl(dbUser, "/users/index/" + dbUser.Id.ToString());

            return this.Redirect(donateUrl);
        }

        private static string GenerateDonateUrl(ApplicationUser user, string returnToUrl)
        {
            StringBuilder donateUrl = new StringBuilder();
            donateUrl.Append("https://www.paypal.com/cgi-bin/webscr?cmd=_donations");
            donateUrl.Append($"&business={user.PayPalEmail}");
            donateUrl.Append($"&item_name=Donate {user.UserName} {GlobalConstants.SystemName}");
            donateUrl.Append($"&currency_code=USD");
            donateUrl.Append($"&return={returnToUrl}");
            return donateUrl.ToString();
        }
    }
}
