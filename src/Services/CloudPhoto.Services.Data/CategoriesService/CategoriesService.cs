namespace CloudPhoto.Services.Data.CategoriesService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Mapping;

    public class CategoriesService : ICategoriesService
    {
        private readonly IDeletableEntityRepository<Category> categoriesRepository;

        public CategoriesService(
            IDeletableEntityRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public async Task<string> CreateAsync(string name, string description, string userId)
        {
            var post = new Category
            {
                Description = description,
                Name = name,
                AuthorId = userId,
            };

            await this.categoriesRepository.AddAsync(post);
            await this.categoriesRepository.SaveChangesAsync();
            return post.Id;
        }

        public async Task<bool> Delete(string id)
        {
            var category = this.GetByCategoryId<Category>(id);
            if (category == null)
            {
                return false;
            }

            this.categoriesRepository.Delete(category);
            int result = await this.categoriesRepository.SaveChangesAsync();
            return result == 1;
        }

        public IEnumerable<T> GetAll<T>()
        {
            IQueryable<Category> query =
                this.categoriesRepository.All().OrderBy(x => x.SortOrder);

            return query.To<T>().ToList();
        }

        public T GetByCategoryId<T>(string categoryId)
        {
            IQueryable<Category> query =
                this.categoriesRepository.All()
                .Where(c => c.Id == categoryId)
                .OrderBy(x => x.SortOrder);

            return query.To<T>().FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(string id, string name, string description)
        {
            var category = this.GetByCategoryId<Category>(id);

            if (category == null)
            {
                return false;
            }

            category.Name = name;
            category.Description = description;

            this.categoriesRepository.Update(category);
            int result = await this.categoriesRepository.SaveChangesAsync();
            return result == 1;
        }
    }
}
