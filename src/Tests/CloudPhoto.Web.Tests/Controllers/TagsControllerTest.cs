namespace CloudPhoto.Web.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Web.Controllers;
    using CloudPhoto.Web.ViewModels.Tags;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class TagsControllerTest
    {
        [Fact]
        public void CheckControllerDecoratedWithValidateAntiForgeryToken()
        {
            var mockTagsService = new Mock<ITagsService>();

            TagsController controller = new TagsController(
             mockTagsService.Object);

            var type = controller.GetType();
            var methodInfo = type.GetMethod("AutoCompleteSearch", new Type[] { typeof(string) });
            var attributes = methodInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true);
            Assert.True(attributes.Any(), "No ValidateAntiForgeryTokenAttribute found on AutoCompleteSearch method");
        }

        [Fact]
        public void ValidateInputModel()
        {
            var mockTagsService = new Mock<ITagsService>();

            TagsController controller = new TagsController(
             mockTagsService.Object);

            IActionResult result = controller.AutoCompleteSearch(null);
            Assert.IsType<BadRequestResult>(result);

            result = controller.AutoCompleteSearch("te");
            Assert.IsType<BadRequestResult>(result);

            result = controller.AutoCompleteSearch("tes");
            Assert.IsNotType<BadRequestResult>(result);
        }

        [Fact]
        public void TestFunctionality()
        {
            var mockTagsService = new Mock<ITagsService>();
            mockTagsService.Setup(x => x.FiterTagsByNames<SearchTagDataModel>("nature"))
                .Returns(new System.Collections.Generic.List<SearchTagDataModel>()
                {
                   new SearchTagDataModel()
                   {
                        Id = "1",
                        Name = "nature",
                   },
                   new SearchTagDataModel()
                   {
                        Id = "2",
                        Name = "nature background",
                   },
                });
            mockTagsService.Setup(x => x.FiterTagsByNames<SearchTagDataModel>("people"))
                .Returns(new System.Collections.Generic.List<SearchTagDataModel>()
                {
                   new SearchTagDataModel()
                   {
                        Id = "1",
                        Name = "people",
                   },
                });

            TagsController controller = new TagsController(
             mockTagsService.Object);

            IActionResult result = controller.AutoCompleteSearch("nature");

            Assert.IsType<JsonResult>(result);
            JsonResult jsonResult = (JsonResult)result;

            List<SearchTagDataModel> responseData = (List<SearchTagDataModel>)jsonResult.Value;
            Assert.Equal(2, responseData.Count);

            result = controller.AutoCompleteSearch("people");

            Assert.IsType<JsonResult>(result);
            jsonResult = (JsonResult)result;

            responseData = (List<SearchTagDataModel>)jsonResult.Value;
            Assert.Single(responseData);

            Assert.Equal("people", responseData[0].Name);
        }
    }
}
