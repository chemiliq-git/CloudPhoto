namespace CloudPhoto.Web.Controllers
{
    using System.Threading.Tasks;

    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.VotesService;
    using CloudPhoto.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class VotesController : Controller
    {
        public VotesController(
            UserManager<ApplicationUser> userManager,
            IVotesService votesService,
            IImagesService imagesService)
        {
            this.UserManager = userManager;
            this.VotesService = votesService;
            this.ImagesService = imagesService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public IVotesService VotesService { get; }

        public IImagesService ImagesService { get; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<VoteResponseModel>> Index(VoteInputModel input)
        {
            if (this.ModelState.IsValid)
            {
                if (input == null)
                {
                    return this.BadRequest();
                }

                var userId = this.UserManager.GetUserId(this.User);
                if (string.IsNullOrEmpty(userId))
                {
                    return this.BadRequest();
                }

                if (string.IsNullOrEmpty(input.ImageId))
                {
                    return this.BadRequest();
                }

                var dbImage = this.ImagesService.GetImageById<Image>(input.ImageId);
                if (dbImage == null)
                {
                    return this.BadRequest();
                }

                bool result = await this.VotesService.VoteAsync(input.ImageId, userId, input.IsLike);
                return new VoteResponseModel { Result = result };
            }

            return this.View();
        }
    }
}
