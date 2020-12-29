namespace CloudPhoto.Web.Controllers
{
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly ICategoriesService categoriesService;
        private readonly UserManager<ApplicationUser> userManager;

        public CategoriesController(
            ApplicationDbContext context,
            ICategoriesService categoriesService,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.categoriesService = categoriesService;
            this.userManager = userManager;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var categories = this.categoriesService.GetAll<ListCategoryViewModel>();
            return this.View(categories);
        }

        // GET: Categories/Create
        [Authorize]
        public IActionResult Create()
        {
            this.ViewData["AuthorId"] = new SelectList(this.context.Users, "Id", "Id");
            return this.View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateCategoryViewModel category)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (this.ModelState.IsValid)
            {
                await this.categoriesService.CreateAsync(category.Name, category.Description, user.Id);
                await this.context.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var category = this.categoriesService.GetByCategoryId<EditCategoryViewModel>(id);
            if (category == null)
            {
                return this.NotFound();
            }

            return this.View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(string id, EditCategoryViewModel category)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    bool result = await this.categoriesService.UpdateAsync(id, category.Name, category.Description);
                    if (!result)
                    {
                        return this.NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (this.categoriesService.GetByCategoryId<Category>(id) == null)
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

            return this.View(category);
        }

        // GET: Categories/Delete/5
        [Authorize]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var category = this.categoriesService.GetByCategoryId<ReadonlyCategoryViewMode>(id);
            if (category == null)
            {
                return this.NotFound();
            }

            return this.View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (await this.categoriesService.Delete(id))
            {
                return this.RedirectToAction(nameof(this.Index));
            }
            else
            {
                return this.NotFound();
            }
        }
    }
}
