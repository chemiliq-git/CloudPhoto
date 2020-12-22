﻿namespace CloudPhoto.Services.Data.CategoriesService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Services.Mapping;
    using Microsoft.Extensions.Logging;

    public class CategoriesService : ICategoriesService
    {
        private readonly ILogger<CategoriesService> logger;
        private readonly IDeletableEntityRepository<Category> categoriesRepository;
        private readonly IRepository<Vote> voteRepository;
        private readonly IRepository<Image> imageRepository;
        private readonly IRepository<ImageCategory> imageCategoryRepository;

        public CategoriesService(
            ILogger<CategoriesService> logger,
            IDeletableEntityRepository<Category> categoriesRepository,
            IRepository<Vote> voteRepository,
            IRepository<Image> imageRepository,
            IRepository<ImageCategory> imageCategoryRepository)
        {
            this.logger = logger;
            this.categoriesRepository = categoriesRepository;
            this.voteRepository = voteRepository;
            this.imageRepository = imageRepository;
            this.imageCategoryRepository = imageCategoryRepository;
        }

        public async Task<string> CreateAsync(string name, string description, string userId)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

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
            return result > 0;
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
            try
            {
                var selectTopVoteCategory = (from category in this.categoriesRepository.All()
                                             let likeCounts = (from vote in this.voteRepository.All()
                                                               join image in this.imageRepository.All()
                                                               on vote.ImageId equals image.Id
                                                               join imageCategory in this.imageCategoryRepository.All()
                                                               on image.Id equals imageCategory.ImageId
                                                               where imageCategory.CategoryId == category.Id
                                                               select vote.IsLike).Sum()
                                             select new
                                             {
                                                 Category = category,
                                                 LikeCounts = likeCounts,
                                             })
                         .OrderByDescending(x => x.LikeCounts);

                return selectTopVoteCategory.Select(s => s.Category).Take(countTopCategory).To<T>().ToList();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, $"Error get top vote categories");
                return null;
            }
        }

        public async Task<bool> UpdateAsync(string id, string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var category = this.GetByCategoryId<Category>(id);

            if (category == null)
            {
                return false;
            }

            category.Name = name;
            category.Description = description;

            this.categoriesRepository.Update(category);
            int result = await this.categoriesRepository.SaveChangesAsync();
            return result > 0;
        }
    }
}
