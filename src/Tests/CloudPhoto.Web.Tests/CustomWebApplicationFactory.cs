namespace CloudPhoto.Web.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    using CloudPhoto.Data;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            string configPath = GetConfigFileDependenceOnEnvironment();

            RemoveStandartConfigProviders(builder, configPath);

            builder.ConfigureServices(services =>
            {
                // found standart DBContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));

                // remove standart DBContext
                services.Remove(descriptor);

                // add memory DBContext to work tests
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();
                }
            });
        }

        private static void RemoveStandartConfigProviders(IWebHostBuilder builder, string configPath)
        {
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                IConfigurationSource configProvider = conf.Sources.FirstOrDefault(temp => temp.GetType() == typeof(JsonConfigurationSource));
                while (configProvider != null)
                {
                    conf.Sources.Remove(configProvider);
                    configProvider = conf.Sources.FirstOrDefault(temp => temp.GetType() == typeof(JsonConfigurationSource));
                }

                conf.AddJsonFile(configPath);
            });
        }

        /// <summary>
        /// For local test use appsettings.Development.json.
        /// For github action test use appsettings.json.
        /// </summary>
        /// <returns></returns>
        private static string GetConfigFileDependenceOnEnvironment()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            var configPath = Path.Combine(workingDirectory, "appsettings.json");
            if (File.Exists(Path.Combine(projectDirectory, "appsettings.Development.json")))
            {
                configPath = Path.Combine(projectDirectory, "appsettings.Development.json");
            }

            return configPath;
        }
    }
}