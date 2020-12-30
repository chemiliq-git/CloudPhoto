namespace CloudPhoto.Web.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using CloudPhoto.Common;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Web.ViewModels;
    using CloudPhoto.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class HomeController : BaseController
    {
        public HomeController(
            IConfiguration configuration,
            ICategoriesService categoriesService,
            IImagesService imagesService)
        {
            this.Configuration = configuration;
            this.CategoriesService = categoriesService;
            this.ImagesService = imagesService;
        }

        public IConfiguration Configuration { get; }

        public ICategoriesService CategoriesService { get; }

        public IImagesService ImagesService { get; }

        public IActionResult Index()
        {
            if (!int.TryParse(this.Configuration.GetSection(GlobalConstants.ShowMostLikeCategoryCount).Value, out int countShowCategory))
            {
                countShowCategory = 2;
            }

            if (!int.TryParse(this.Configuration.GetSection(GlobalConstants.ShowMostLikeImageByCategoryCount).Value, out int countShowImages))
            {
                countShowImages = 4;
            }

            List<HomeIndexViewModel> model = new List<HomeIndexViewModel>();

            IEnumerable<CategoryHomeViewModel> topCategory = this.CategoriesService.GetMostLikedCategory<CategoryHomeViewModel>(countShowCategory);
            HomeIndexViewModel tempModel;
            foreach (CategoryHomeViewModel category in topCategory)
            {
                tempModel = new HomeIndexViewModel
                {
                    CategoryInfo = category,
                    CategoryImages = this.ImagesService.GetMostLikeImageByCategory<ImageHomeViewModel>(category.Id, countShowImages)?.ToList(),
                };
                model.Add(tempModel);
            }

            return this.View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
