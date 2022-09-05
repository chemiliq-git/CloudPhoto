namespace CloudPhoto.Web.Controllers
{
    using System.Threading.Tasks;

    using Data.Models;
    using CloudPhoto.Services.Data.SubscribesService;
    using ViewModels.Subscribes;
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
            UserManager = userManager;
            SubscribesService = subscribesService;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        public ISubscribesService SubscribesService { get; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<SubscribeResponseModel>> Subscribe(SubscribeInputModel input)
        {
            if (!ModelState.IsValid
                || input == null
                || string.IsNullOrEmpty(input.UserId))
            {
                return BadRequest();
            }

            ApplicationUser userToSubscribe = await UserManager.FindByIdAsync(input.UserId);
            if (userToSubscribe == null)
            {
                return BadRequest();
            }

            var userSubcrebed = UserManager.GetUserId(User);

            if (string.Compare(userToSubscribe.Id.ToString(), userSubcrebed, true) == 0)
            {
                return BadRequest();
            }

            bool result = await SubscribesService.ManageUserSubsctibe(userSubcrebed, userToSubscribe.Id, input.Follow);
            return new SubscribeResponseModel { Result = result };
        }
    }
}
