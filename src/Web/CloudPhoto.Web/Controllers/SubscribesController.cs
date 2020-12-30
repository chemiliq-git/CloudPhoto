namespace CloudPhoto.Web.Controllers
{
    using System.Threading.Tasks;

    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.SubscribesService;
    using CloudPhoto.Web.ViewModels.Subscribes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class SubscribesController : BaseController
    {
        public SubscribesController(
            UserManager<ApplicationUser> userManager,
            ISubscribesService subscribesService)
        {
            this.UserManager = userManager;
            this.SubscribesService = subscribesService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public ISubscribesService SubscribesService { get; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<SubscribeResponseModel>> Subscribe(SubscribeInputModel input)
        {
            if (!this.ModelState.IsValid
                || input == null
                || string.IsNullOrEmpty(input.UserId))
            {
                return this.BadRequest();
            }

            ApplicationUser userToSubscribe = await this.UserManager.FindByIdAsync(input.UserId);
            if (userToSubscribe == null)
            {
                return this.BadRequest();
            }

            var userSubcrebed = this.UserManager.GetUserId(this.User);

            if (string.Compare(userToSubscribe.Id.ToString(), userSubcrebed, true) == 0)
            {
                return this.BadRequest();
            }

            bool result = await this.SubscribesService.ManageUserSubsctibe(userSubcrebed, userToSubscribe.Id, input.Follow);
            return new SubscribeResponseModel { Result = result };
        }
    }
}
