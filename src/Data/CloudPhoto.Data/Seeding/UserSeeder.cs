namespace CloudPhoto.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using CloudPhoto.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal class UserSeeder : ISeeder
    {
        public UserSeeder(ILogger logger)
        {
            this.Logger = logger;
        }

        public ILogger Logger { get; }

        public async Task SeedAsync(
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            ApplicationRole administrator = await roleManager.FindByNameAsync(GlobalConstants.AdministratorRoleName);
            ApplicationUser user = await userManager.FindByNameAsync(GlobalConstants.DefaultAdministratorEmail);
            if (administrator != null)
            {
                if (user == null)
                {
                    ApplicationUser admin = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = GlobalConstants.DefaultAdministratorEmail,
                        Email = GlobalConstants.DefaultAdministratorEmail,
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(admin, GlobalConstants.DefaultAdministratorPassword);

                    dbContext.UserRoles.Add(new IdentityUserRole<string>()
                    {
                        RoleId = administrator.Id,
                        UserId = admin.Id,
                    });
                }
            }
            else
            {
                this.Logger.LogError($"Not found user with role {GlobalConstants.AdministratorRoleName}");
                return;
            }
        }
    }
}
