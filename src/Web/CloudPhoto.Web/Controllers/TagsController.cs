namespace CloudPhoto.Web.Controllers
{
    using System.Threading.Tasks;
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Web.ViewModels.Tags;
    using Microsoft.AspNetCore.Mvc;

    public class TagsController : Controller
    {

        public TagsController(ITagsService tagsService)
        {
            this.TagsService = tagsService;
        }

        public ITagsService TagsService { get; }

        [HttpPost]
        public IActionResult AutoCompleteSearch(string searchData)
        {
            var foundTags = this.TagsService.FiterTagsByNames<SearchTagDataModel>(searchData);
            return this.Json(foundTags);
        }
    }
}
