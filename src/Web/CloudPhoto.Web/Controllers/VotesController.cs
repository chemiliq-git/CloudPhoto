namespace CloudPhoto.Web.Controllers
{
    using System.Threading.Tasks;

    using Data.Models;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.VotesService;
    using ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class VotesController : BaseController
    {
        public VotesController(
            UserManager<ApplicationUser> userManager,
            IVotesService votesService,
            IImagesService imagesService)
        {
            UserManager = userManager;
            VotesService = votesService;
            ImagesService = imagesService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public IVotesService VotesService { get; }

        public IImagesService ImagesService { get; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<VoteResponseModel>> Index(VoteInputModel input)
        {
            if (ModelState.IsValid)
            {
                if (input == null)
                {
                    return BadRequest();
                }

                var userId = UserManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest();
                }

                if (string.IsNullOrEmpty(input.ImageId))
                {
                    return BadRequest();
                }

                var dbImage = ImagesService.GetImageById<Image>(input.ImageId);
                if (dbImage == null)
                {
                    return BadRequest();
                }

                bool result = await VotesService.VoteAsync(input.ImageId, userId, input.IsLike);
                return new VoteResponseModel { Result = result };
            }

            return View();
        }
    }
}
