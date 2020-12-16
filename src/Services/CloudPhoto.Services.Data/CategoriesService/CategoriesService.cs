namespace CloudPhoto.Services.Data.CategoriesService
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Data.DapperService;
    using CloudPhoto.Services.Mapping;

    public class CategoriesService : ICategoriesService
    {
        private readonly IDeletableEntityRepository<Category> categoriesRepository;

        private readonly IDapperService dapperService;

        public CategoriesService(
            IDeletableEntityRepository<Category> categoriesRepository,
            IDapperService dapperService)
        {
            this.categoriesRepository = categoriesRepository;
            this.dapperService = dapperService;
        }

        public async Task<string> CreateAsync(string name, string description, string userId)
        {
            var category = new Category
            {
                Description = description,
                Name = name,
                AuthorId = userId,
            };

            await this.categoriesRepository.AddAsync(category);
            await this.categoriesRepository.SaveChangesAsync();
            return category.Id;
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

        public IEnumerable<T> GetMostLikedCategory<T>(int countTopCategory)
        {
            StringBuilder sqlSelect = new StringBuilder(
                @"SELECT TOP(@countTopCategory) *,
                (SELECT SUM(v.IsLike) from Votes AS v
                JOIN Images AS i ON i.id = v.ImageId
                JOIN ImageCategories AS ic ON ic.ImageId = i.Id
                WHERE ic.CategoryId = c.Id) AS likeCounts
                FROM Categories AS c
                ORDER BY likeCounts DESC");
            var parameters = new
            {
                countTopCategory,
            };
            return this.dapperService.GetAll<T>(sqlSelect.ToString(), parameters, commandType: CommandType.Text);
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
