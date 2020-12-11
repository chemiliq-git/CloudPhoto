﻿namespace CloudPhoto.Web.Controllers
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

                string newId = await this.imagesService.CreateAsync(
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

                if (string.IsNullOrEmpty(newId))
                {
                    image.Categories = this.categoriesService.GetAll<CategoryDropDownViewModel>();
                    return this.View(image);
                }
                else
                {
                    return this.RedirectToAction(nameof(this.Index));
                }
            }

            image.Categories = this.categoriesService.GetAll<CategoryDropDownViewModel>();
            return this.View(image);
        }

        [HttpPost(Name = "PreviewImage")]
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

            if (this.User.Identity.IsAuthenticated)
            {
                ApplicationUser user = await this.userManager.GetUserAsync(this.User);
                localSearchData.LikeForUserId = user.Id;
            }

            var data = this.imagesService.GetByFilter<ImagePreviewViewModel>(
                    localSearchData, 1, id);

            if (!data.Any())
            {
                return this.Json(string.Empty);
            }
            else
            {
                ImagePreviewViewModel previewImage = data.First();
                previewImage.ImageIndex = id;

                return this.PartialView("_PreviewImagePartial", previewImage);
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
            int pageIndex,
            int pageSize,
            string searchText,
            string selectCategory)
        {
            SearchImageData localSearchData = new SearchImageData
            {
                FilterByTag = searchText,
            };
            if (!string.IsNullOrEmpty(selectCategory))
            {
                localSearchData.FilterCategory = JsonSerializer.Deserialize<List<string>>(selectCategory);
            }

            if (pageSize == 0)
            {
                return this.BadRequest();
            }

            if (pageIndex == 0)
            {
                return this.BadRequest();
            }

            if (this.User.Identity.IsAuthenticated)
            {
                ApplicationUser user = await this.userManager.GetUserAsync(this.User);
                localSearchData.LikeForUserId = user.Id;
            }

            var data = this.imagesService.GetByFilter<ListImageViewModel>(
                localSearchData, pageSize, pageIndex);

            int indexOfPage = 1;
            foreach (ListImageViewModel model in data)
            {
                model.ImageIndex = ((pageIndex - 1) * pageSize) + indexOfPage;
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

        public IActionResult PhotoSwipe()
        {
            return this.View();
        }

        public IActionResult Fotorama()
        {
            return this.View();
        }

        public IActionResult Modal()
        {
            return this.View();
        }

        private bool ImageExists(string id)
        {
            return this.context.Images.Any(e => e.Id == id);
        }
    }
}
