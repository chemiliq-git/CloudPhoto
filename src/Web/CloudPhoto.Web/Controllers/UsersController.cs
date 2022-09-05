namespace CloudPhoto.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Common;
    using Data.Models;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.UsersServices;
    using CloudPhoto.Services.Data.VotesService;
    using ViewModels.Images;
    using ViewModels.Users;
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
            UserManager = userManager;
            UsersServices = usersServices;
            ImagesService = imagesService;
            VotesService = votesService;
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
                return BadRequest();
            }

            ApplicationUser loggedUser = await UserManager.GetUserAsync(User);

            ApplicationUser previewUser = await UserManager.FindByIdAsync(id);
            if (previewUser == null)
            {
                return BadRequest();
            }

            UserPreviewViewModel userInfo = UsersServices.GetUserInfo<UserPreviewViewModel>(id, loggedUser?.Id);
            return View(userInfo);
        }

        [HttpPost("GetLinkedUsers")]
        public async Task<IActionResult> GetLinkedUsers(
            int pageIndex,
            int pageSize,
            string userId,
            string type)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
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
                        return BadRequest();
                    }
            }

            string likeForUserId = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser loginUser = await UserManager.GetUserAsync(User);
                likeForUserId = loginUser.Id;
            }

            IEnumerable<UserListViewModel> lstUsersInfo;
            if (isSearchFollowing)
            {
                lstUsersInfo = UsersServices.GetFollowingUsers<UserListViewModel>(
                          userId,
                          likeForUserId,
                          pageSize,
                          pageIndex);
            }
            else
            {
                lstUsersInfo = UsersServices.GetFollowerUsers<UserListViewModel>(
                        userId,
                        likeForUserId,
                        pageSize,
                        pageIndex);
            }

            return PartialView("_UserListPartial", lstUsersInfo);
        }

        [HttpPost("UpdateAvatar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(string userId, string avatarUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(avatarUrl))
            {
                return BadRequest();
            }

            ApplicationUser user = await UserManager.GetUserAsync(User);
            if (user.Id != userId)
            {
                return BadRequest();
            }

            return Json(await UsersServices.ChangeAvatar(userId, avatarUrl));
        }

        [HttpGet("Donate")]
        public async Task<IActionResult> Donate(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            ApplicationUser dbUser = await UserManager.FindByIdAsync(id);
            if (dbUser == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(dbUser.PayPalEmail))
            {
                return BadRequest();
            }

            string donateUrl = GenerateDonateUrl(dbUser, "/users/index/" + dbUser.Id.ToString());

            return Redirect(donateUrl);
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
