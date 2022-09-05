namespace CloudPhoto.Web.Controllers
{
    using System.Threading.Tasks;

    using Common;
    using Data;
    using Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using ViewModels.Categories;
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
            var categories = categoriesService.GetAll<ListCategoryViewModel>();
            return View(categories);
        }

        // GET: Categories/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(context.Users, "Id", "Id");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateCategoryViewModel category)
        {
            var user = await userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                await categoriesService.CreateAsync(category.Name, category.Description, user.Id);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = categoriesService.GetByCategoryId<EditCategoryViewModel>(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(string id, EditCategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool result = await categoriesService.UpdateAsync(id, category.Name, category.Description);
                    if (!result)
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (categoriesService.GetByCategoryId<Category>(id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = categoriesService.GetByCategoryId<ReadonlyCategoryViewMode>(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (await categoriesService.Delete(id))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
