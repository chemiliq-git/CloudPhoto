namespace CloudPhoto.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using CloudPhoto.Common;
    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Web.ViewModels.Categories;
    using CloudPhoto.Web.ViewModels.FilterBar;
    using CloudPhoto.Web.ViewModels.Images;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;
        private readonly UserManager<ApplicationUser> userManager;

        public ImagesController(
            ApplicationDbContext context,
            IImagesService imagesService,
            ICategoriesService categoriesService,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.imagesService = imagesService;
            this.categoriesService = categoriesService;
            this.userManager = userManager;
        }

        // GET: Images
        public IActionResult Index(int page = 1, int perPage = GlobalConstants.ImagePerPageDefaultValue)
        {
            var images = this.imagesService.GetByFilter<ListImageViewModel>(
                new SearchImageData(),
                page,
                perPage);
            return this.View(images);
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var image = await this.context.Images
                .Include(i => i.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return this.NotFound();
            }

            return this.View(image);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            CreateImageViewModel model = new CreateImageViewModel();
            model.Categories = this.categoriesService.GetAll<CategoryDropDownViewModel>();
            return this.View(model);
        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateImageViewModel image)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.GetUserAsync(this.User);

                List<string> lstImageTag = JsonSerializer.Deserialize<List<string>>(image.ImageTags);

                await this.imagesService.CreateAsync(
                     new CreateImageModelData()
                     {
                         Title = image.Title,
                         Description = image.Description,
                         CategoryId = image.CategoryId,
                         ImageUrl = image.ImageUrl,
                         AuthorId = user.Id,
                         Tags = lstImageTag,
                     }
               );

                return this.RedirectToAction(nameof(this.Index));
            }

            image.Categories = this.categoriesService.GetAll<CategoryDropDownViewModel>();
            return this.View(image);
        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var image = await this.context.Images.FindAsync(id);
            if (image == null)
            {
                return this.NotFound();
            }

            return this.View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Image image)
        {
            if (id != image.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this.context.Update(image);
                    await this.context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.ImageExists(image.Id))
                    {
                        return this.NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(image);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var image = await this.context.Images
                .Include(i => i.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return this.NotFound();
            }

            return this.View(image);
        }

        // POST: Images/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var image = await this.context.Images.FindAsync(id);
            this.context.Images.Remove(image);
            await this.context.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> GetSearchingData(FilterBarSearchDataViewModel inputSearchData)

        public async Task<ActionResult> GetSearchingData(int page,int perPage)
        {
            SearchImageData searchData = new SearchImageData();
            //if (inputSearchData.Category != null)
            //{
            //    searchData.FilterCategory = inputSearchData.Category.Where(i => i.Check).Select(o => o.Id).ToList();
            //}


            if (this.User.Identity.IsAuthenticated)
            {
                var user = await this.userManager.GetUserAsync(this.User);
                searchData.AuthorId = user.Id;
            }

            var data = this.imagesService.GetByFilter<ListImageViewModel>(
                searchData, page, perPage);
            //inputSearchData.Page == 0 ? 1 : inputSearchData.Page,
            //inputSearchData.PerPage == 0 ? GlobalConstants.ImagePerPageDefaultValue : inputSearchData.PerPage);
            
            return this.PartialView("_ImageListPartial", data);
            
            //return this.View("Index", data);
            
            //string message = "true";
            //return this.Json(message);
        }

        private bool ImageExists(string id)
        {
            return this.context.Images.Any(e => e.Id == id);
        }
    }
}
