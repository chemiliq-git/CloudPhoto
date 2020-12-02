namespace CloudPhoto.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.RegularExpressions;
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
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment env;

        public ImagesController(
            ApplicationDbContext context,
            IConfiguration configuration,
            IImagesService imagesService,
            ICategoriesService categoriesService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            this.context = context;
            this.configuration = configuration;
            this.imagesService = imagesService;
            this.categoriesService = categoriesService;
            this.userManager = userManager;
            this.env = env;
        }

        // GET: Images
        public IActionResult Index()
        {
            return this.View();
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
            CreateImageViewModel model = new CreateImageViewModel
            {
                Categories = this.categoriesService.GetAll<CategoryDropDownViewModel>(),
            };
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
                    this.env.WebRootPath,
                    new CreateImageModelData()
                    {
                        Id = image.ImageId,
                        Title = image.Title,
                        Description = image.Description,
                        CategoryId = image.CategoryId,
                        ImageUrl = image.ImageUrl,
                        AuthorId = user.Id,
                        Tags = lstImageTag,
                    });

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
        public async Task<ActionResult> GetSearchingData(int page, int perPage, string searchData)
        {
            SearchImageData localSearchData = new SearchImageData();
            localSearchData.FilterCategory = this.ParseSelectCategories(searchData);

            if (perPage == 0)
            {
                return this.BadRequest();
            }

            if (page == 0)
            {
                return this.BadRequest();
            }

            if (this.User.Identity.IsAuthenticated)
            {
                var user = await this.userManager.GetUserAsync(this.User);
                localSearchData.AuthorId = user.Id;
            }

            var data = this.imagesService.GetByFilter<ListImageViewModel>(
                localSearchData, perPage, page);

            if (!data.Any())
            {
                return this.Json(string.Empty);
            }
            else
            {
                return this.PartialView("_ImageListPartial", data);
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> GetPageCount(int itemPerPage, string searchData)
        {
            SearchImageData localSearchData = new SearchImageData();
            localSearchData.FilterCategory = this.ParseSelectCategories(searchData);

            if (itemPerPage == 0)
            {
                return this.BadRequest();
            }

            if (this.User.Identity.IsAuthenticated)
            {
                var user = await this.userManager.GetUserAsync(this.User);
                localSearchData.AuthorId = user.Id;
            }

            int count = this.imagesService.GetCountByFilter<ListImageViewModel>(localSearchData);
            return count == 0 ? 0 : (int)Math.Ceiling((double)count / itemPerPage);
        }

        private List<string> ParseSelectCategories(string searchData1)
        {
            List<string> selectCategories = new List<string>();
            string regex = "checkBoxValue=true&Category%5B[0-9]*%5D.Id=(?<id>[0-9a-z]{8}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{12})";
            MatchCollection matches = Regex.Matches(searchData1, regex);
            foreach (Match item in matches)
            {
                selectCategories.Add(item.Groups["id"].Value);
            }

            return selectCategories;
        }

        private bool ImageExists(string id)
        {
            return this.context.Images.Any(e => e.Id == id);
        }
    }
}
