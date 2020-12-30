namespace CloudPhoto.Web.Controllers
{
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Web.ViewModels.Tags;
    using Microsoft.AspNetCore.Mvc;

    public class TagsController : BaseController
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
            if (string.IsNullOrEmpty(searchData))
            {
                return this.BadRequest();
            }

            if (searchData.Length <= 2)
            {
                return this.BadRequest();
            }

            var foundTags = this.TagsService.FiterTagsByNames<SearchTagDataModel>(searchData);
            return this.Json(foundTags);
        }
    }
}
