namespace CloudPhoto.Web.Controllers
{
    using CloudPhoto.Services.Data.TagsService;
    using ViewModels.Tags;
    using Microsoft.AspNetCore.Mvc;

    public class TagsController : BaseController
    {
        public TagsController(ITagsService tagsService)
        {
            TagsService = tagsService;
        }

        public ITagsService TagsService { get; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AutoCompleteSearch(string searchData)
        {
            if (string.IsNullOrEmpty(searchData))
            {
                return BadRequest();
            }

            if (searchData.Length < 2)
            {
                return BadRequest();
            }

            var foundTags = TagsService.FiterTagsByNames<SearchTagDataModel>(searchData);
            return Json(foundTags);
        }
    }
}
