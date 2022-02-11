using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace AutoAdminLib.IntegrationTest.Setup {
    public class TestFactory : WebApplicationFactory<AutoAdminLib.TestWebApp.Startup> {
        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            base.ConfigureWebHost(builder);
            builder.ConfigureAppConfiguration((_, config) => {
                config.AddJsonFile("appsettings.IntegrationTest.json");
            });
        }
    }
}
