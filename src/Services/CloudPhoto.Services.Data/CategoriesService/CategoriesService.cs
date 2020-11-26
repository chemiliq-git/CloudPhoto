namespace CloudPhoto.Services.Data.CategoriesService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
    }
}
