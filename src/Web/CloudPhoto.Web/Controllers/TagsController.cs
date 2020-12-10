namespace CloudPhoto.Web.Controllers
{
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
        [ValidateAntiForgeryToken]
        public IActionResult AutoCompleteSearch(string searchData)
        {
            var foundTags = this.TagsService.FiterTagsByNames<SearchTagDataModel>(searchData);
            return this.Json(foundTags);
        }
    }
}
