using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace AutoAdmin.IntegrationTest.Setup {
    public class TestFactory : WebApplicationFactory<Startup> {
        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            base.ConfigureWebHost(builder);
            builder.ConfigureAppConfiguration((_, config) => {
                config.AddJsonFile("appsettings.IntegrationTest.json");
            });
        }
    }
}
