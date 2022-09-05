namespace CloudPhoto.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal class CategorySeeder : ISeeder
    {
        public CategorySeeder(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any())
            {
                return;
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            ApplicationUser admin = await userManager.FindByEmailAsync(GlobalConstants.DefaultAdministratorEmail);
            if (admin == null)
            {
                Logger.LogError($"Not found administartor with email: {GlobalConstants.DefaultAdministratorEmail}");
                return;
            }

            List<Category> lstCategories = new List<Category>
            {
                new Category
                {
                    Name = "Wallpepers",
                    Description = "Wallpepers",
                    AuthorId = admin.Id,
                    SortOrder = 1,
                },

                new Category
                {
                    Name = "Animals",
                    Description = "Animals",
                    AuthorId = admin.Id,
                    SortOrder = 2,
                },

                new Category
                {
                    Name = "Seasons",
                    Description = "Seasons",
                    AuthorId = admin.Id,
                    SortOrder = 3,
                },

                new Category
                {
                    Name = "Nature",
                    Description = "Nature",
                    AuthorId = admin.Id,
                    SortOrder = 4,
                },

                new Category
                {
                    Name = "Cars",
                    Description = "Cars",
                    AuthorId = admin.Id,
                    SortOrder = 5,
                },

                new Category
                {
                    Name = "Outdoors",
                    Description = "Outdoors",
                    AuthorId = admin.Id,
                    SortOrder = 6,
                },

                new Category
                {
                    Name = "Backgrounds",
                    Description = "Backgrounds",
                    AuthorId = admin.Id,
                    SortOrder = 7,
                },

                new Category
                {
                    Name = "Road",
                    Description = "Road",
                    AuthorId = admin.Id,
                    SortOrder = 8,
                },

                new Category
                {
                    Name = "City",
                    Description = "City",
                    AuthorId = admin.Id,
                    SortOrder = 9,
                },

                new Category
                {
                    Name = "Game",
                    Description = "Game",
                    AuthorId = admin.Id,
                    SortOrder = 10,
                },
            };

            await dbContext.Categories.AddRangeAsync(lstCategories);
        }
    }
}
