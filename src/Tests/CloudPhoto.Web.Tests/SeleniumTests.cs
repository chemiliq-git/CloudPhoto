namespace CloudPhoto.Web.Tests
{
    using System;
    using System.Linq;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using Xunit;

    public class SeleniumTests : IClassFixture<SeleniumServerFactory<Startup>>, IDisposable
    {
        private readonly SeleniumServerFactory<Startup> server;
        private readonly IWebDriver browser;

        public SeleniumTests(SeleniumServerFactory<Startup> server)
        {
            this.server = server;
            server.CreateClient();
            var opts = new ChromeOptions();
            opts.AddArguments("--headless");
            opts.AcceptInsecureCertificates = true;
            browser = new ChromeDriver(opts);
        }

        [Fact(Skip = "Example test. Disabled for CI.")]
        public void FooterOfThePageContainsPrivacyLink()
        {
            browser.Navigate().GoToUrl(server.RootUri);
            Assert.EndsWith(
                "/Home/Privacy",
                browser.FindElements(By.CssSelector("footer a")).First().GetAttribute("href"));
        }

        [Fact(Skip ="Not implement")]
        public void HomePageMustContainLoginButton()
        {
            browser.Navigate().GoToUrl(server.RootUri);
            browser.FindElement(By.Id("headerSearchControl")).SendKeys("people" + Keys.Enter);

            WebDriverWait wait = new WebDriverWait(browser, TimeSpan.FromSeconds(10));
            IWebElement firstResult = wait.Until(e => e.FindElement(By.XPath("//a/h3")));

            Console.WriteLine(firstResult.Text);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                server?.Dispose();
                browser?.Dispose();
            }
        }
    }
}
