namespace CloudPhoto.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly ICategoriesService categoriesService;

        public CategoriesController(ApplicationDbContext context, ICategoriesService categoriesService)
        {
            this.context = context;
            this.categoriesService = categoriesService;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var categories = this.categoriesService.GetAll<ListCategoryViewModel>();
            return this.View(categories);
        }

        // GET: Categories/Details/5
        public IActionResult Details(string id)
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

        // GET: Categories/Create
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
        public async Task<IActionResult> Create([Bind("Name,Description,ImageUrl,AuthorId,SortOrder,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Category category)
        {
            if (this.ModelState.IsValid)
            {
                this.context.Add(category);
                await this.context.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            }

            this.ViewData["AuthorId"] = new SelectList(this.context.Users, "Id", "Id", category.AuthorId);
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
        public async Task<IActionResult> Edit(string id, EditCategoryViewModel category)
        {
            if (this.ModelState.IsValid)
            {
                var data = this.categoriesService.GetByCategoryId<Category>(id);

                if (data == null)
                {
                    return this.NotFound();
                }

                data.Description = category.Description;
                data.Name = category.Name;

                try
                {
                    this.context.Update(data);
                    await this.context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.CategoryExists(data.Id))
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
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var category = await this.context.Categories
                .Include(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var category = await this.context.Categories.FindAsync(id);
            this.context.Categories.Remove(category);
            await this.context.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }

        private bool CategoryExists(string id)
        {
            return this.context.Categories.Any(e => e.Id == id);
        }
    }
}
