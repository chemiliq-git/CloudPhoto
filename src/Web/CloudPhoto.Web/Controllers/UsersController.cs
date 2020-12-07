﻿namespace CloudPhoto.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.VotesService;
    using CloudPhoto.Web.ViewModels.Images;
    using CloudPhoto.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class UsersController : Controller
    {
        public UsersController(
            UserManager<ApplicationUser> userManager,
            IImagesService imagesService,
            IVotesService votesService)
        {
            this.UserManager = userManager;
            this.ImagesService = imagesService;
            this.VotesService = votesService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public IImagesService ImagesService { get; }

        public IVotesService VotesService { get; }

        public async Task<IActionResult> Index(string id)
        {
            ApplicationUser user = await this.UserManager.FindByIdAsync(id);

            UserPreviewViewModel model = new UserPreviewViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetPagingData(string id)
        {
            if (!this.Request.Cookies.TryGetValue("pagingData", out string readPagingDataCookie))
            {
                return this.BadRequest();
            }

            ApplicationUser user = await this.UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return this.BadRequest();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            PagingCookieData cookieSearchData = JsonSerializer.Deserialize<PagingCookieData>(readPagingDataCookie, options);


            ApplicationUser loginUser = null;
            if (this.User.Identity.IsAuthenticated)
            {
                loginUser = await this.UserManager.GetUserAsync(this.User);
            }

            SearchImageData localSearchData = null;
            if (cookieSearchData.Type == "uploads")
            {
                localSearchData = new SearchImageData
                {
                    AuthorId = user.Id,
                };
            }
            else if (cookieSearchData.Type == "likes")
            {
                localSearchData = new SearchImageData
                {
                    LikeByUser = user.Id,
                };
            }

            var data = this.ImagesService.GetByFilter<ListImageViewModel>(
                    localSearchData, cookieSearchData.PageSize, cookieSearchData.PageIndex);


            List<Vote> lstVotes = null;
            if (loginUser != null)
            {
                lstVotes = this.VotesService.GetByUser<Vote>(loginUser.Id).ToList();
            }

            int indexOfPage = 1;
            foreach (ListImageViewModel model in data)
            {
                model.ImageIndex = ((cookieSearchData.PageIndex - 1) * cookieSearchData.PageSize) + indexOfPage;
                if (loginUser == null)
                {
                    model.IsLike = false;
                }
                else
                {
                    model.IsLike = lstVotes.Where(temp => temp.ImageId == model.Id).Sum(temp => temp.IsLike) == 1;
                }

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
    }
}
