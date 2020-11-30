namespace CloudPhoto.Web.ViewComponents
{
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Web.ViewModels.Categories;
    using CloudPhoto.Web.ViewModels.FilterBar;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [ViewComponent(Name = "FilterBar")]
    public class FilterBarViewComponent : ViewComponent
    {
        public FilterBarViewComponent(ICategoriesService categoriesService)
        {
            this.CategoriesService = categoriesService;
        }

        public ICategoriesService CategoriesService { get; }

        public IViewComponentResult Invoke()
        {
            FilterBarSearchDataViewModel data = new FilterBarSearchDataViewModel();
            data.Category = (List<CategoryCheckBoxViewModel>)this.CategoriesService.GetAll<CategoryCheckBoxViewModel>();
            return this.View(data);
        }
    }
}
