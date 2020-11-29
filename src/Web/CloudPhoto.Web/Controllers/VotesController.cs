namespace CloudPhoto.Web.Controllers
{
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.VotesService;
    using CloudPhoto.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    public class VotesController : Controller
    {
        public VotesController(
            UserManager<ApplicationUser> userManager,
            IVotesService votesService
            )
        {
           this.UserManager = userManager;
           this.VotesService = votesService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public IVotesService VotesService { get; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<VoteResponseModel>> Index(VoteInputModel input)
        {
            if (this.ModelState.IsValid)
            {
                var userId = this.UserManager.GetUserId(this.User);
                bool result = await this.VotesService.VoteAsync(input.ImageId, userId, input.IsLike);
                return new VoteResponseModel { Result = result };
            }

            return this.View();
        }
    }
}
