namespace CloudPhoto.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.VotesService;
    using CloudPhoto.Web.ViewModels.Categories;
    using CloudPhoto.Web.ViewModels.Images;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
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
        private readonly IVotesService votesService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment env;

        public ImagesController(
            ApplicationDbContext context,
            IConfiguration configuration,
            IImagesService imagesService,
            ICategoriesService categoriesService,
            IVotesService votesService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            this.context = context;
            this.configuration = configuration;
            this.imagesService = imagesService;
            this.categoriesService = categoriesService;
            this.votesService = votesService;
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

        public async Task<IActionResult> PreviewImage(int id)
        {
            if (!this.Request.Cookies.TryGetValue("searchData", out string readSearchDataCookie))
            {
                return this.BadRequest();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            SeachCookieData cookieSearchData = JsonSerializer.Deserialize<SeachCookieData>(readSearchDataCookie, options);

            SearchImageData localSearchData = new SearchImageData
            {
                FilterByTag = cookieSearchData.SearchText,
                FilterCategory = cookieSearchData.SelectCategory,
            };

            ApplicationUser user = null;
            if (this.User.Identity.IsAuthenticated)
            {
                user = await this.userManager.GetUserAsync(this.User);
            }

            var data = this.imagesService.GetByFilter<ImagePreviewViewModel>(
                    localSearchData, 1, id);

            if (!data.Any())
            {
                if (id > 1)
                {
                    data = this.imagesService.GetByFilter<ImagePreviewViewModel>(
                        localSearchData, 1, id - 1);
                    ImagePreviewViewModel previewImage = data.First();
                    previewImage.ImageIndex = id - 1;
                    previewImage.IsEndedImage = true;
                    this.SetIsLikeFlags(user, previewImage);

                    return this.View(previewImage);
                }

                return this.Json(string.Empty);
            }
            else
            {
                ImagePreviewViewModel previewImage = data.First();
                previewImage.ImageIndex = id;
                this.SetIsLikeFlags(user, previewImage);

                return this.View(previewImage);
            }
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetSearchingData(
            int page,
            int perPage,
            string searchText,
            string filterByCategories)
        {
            SearchImageData localSearchData = new SearchImageData
            {
                FilterByTag = searchText,
            };
            if (!string.IsNullOrEmpty(filterByCategories))
            {
                localSearchData.FilterCategory = JsonSerializer.Deserialize<List<string>>(filterByCategories);
            }

            if (perPage == 0)
            {
                return this.BadRequest();
            }

            if (page == 0)
            {
                return this.BadRequest();
            }

            ApplicationUser user = null;
            if (this.User.Identity.IsAuthenticated)
            {
                user = await this.userManager.GetUserAsync(this.User);
            }

            var data = this.imagesService.GetByFilter<ListImageViewModel>(
                localSearchData, perPage, page);

            List<Vote> lstVotes = null;
            if (user != null)
            {
                lstVotes = this.votesService.GetByUser<Vote>(user.Id).ToList();
            }

            int indexOfPage = 1;
            foreach (ListImageViewModel model in data)
            {
                model.ImageIndex = ((page - 1) * perPage) + indexOfPage;
                if (user == null)
                {
                    model.IsLike = false;
                }
                else
                {
                    model.IsLike = lstVotes.Where(temp => temp.ImageId == model.Id).Sum(temp => temp.IsLike) == 1;
                }

                indexOfPage++;
            }

            if (!data.Any())
            {
                return this.Json(string.Empty);
            }
            else
            {
                return this.PartialView("_ImageListPartial", data);
            }
        }

        private void SetIsLikeFlags(ApplicationUser user, ImagePreviewViewModel previewImage)
        {
            List<Vote> lstVotes = this.votesService.GetByImage<Vote>(previewImage.Id).ToList();
            if (user != null)
            {
                previewImage.IsLike = lstVotes.Where(t => t.AuthorId == user.Id && t.IsLike == 1).Any();
            }

            previewImage.LikeCount = lstVotes.Sum(t => t.IsLike);
        }

        private bool ImageExists(string id)
        {
            return this.context.Images.Any(e => e.Id == id);
        }
    }
}
