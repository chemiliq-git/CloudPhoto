namespace CloudPhoto.Web.Tests
{
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class WebTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> server;

        public WebTests(CustomWebApplicationFactory<Startup> server)
        {
            this.server = server;
        }

        // [Fact(Skip = "Example test. Disabled for CI.")]
        [Fact]
        public async Task IndexPageShouldReturnStatusCode200WithTitle()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>", responseContent);
        }

        [Fact]
        public async Task CategoryManagePageRequiresAuthorization()
        {
            var client = this.server.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var response = await client.GetAsync("Administration/Categories");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task CategoryCreatePageRequiresAuthorization()
        {
            var client = this.server.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var response = await client.GetAsync("Administration/Categories/Create");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task CategoryDeletePageRequiresAuthorization()
        {
            var client = this.server.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var response = await client.GetAsync("Administration/Categories/Delete");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task CheckHomePage()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>", responseContent);
        }
    }
}
