using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Extensions.Ordering;
using FluentAssertions;
using AutoAdmin.IntegrationTest.Setup;
using AutoAdmin.Config;

namespace AutoAdmin.IntegrationTest.Injection
{
    [Collection("AutoAdmin Collection"), Order(1)]
    public class DbConfigurationsTest
    {
        private readonly DbConfigurations _configurations;
        public DbConfigurationsTest(TestFactory testFactory)
        {
            testFactory.CreateClient();
            _configurations = testFactory.Server.Services.GetService<IOptions<DbConfigurations>>()?.Value;
        }

        [Fact(DisplayName = "should be bound successfully")]
        public void ShouldBeBoundSuccessfully()
        {
            _configurations.Should().NotBeNull();
            _configurations.ConnectionString.Should().NotBeNullOrWhiteSpace();

            _configurations.Process();
            _configurations.Server.Should().BeEquivalentTo("localhost");
            _configurations.Port.Should().BeEquivalentTo("3306");
            _configurations.DbName.Should().BeEquivalentTo("AutoAdminDb");
            _configurations.User.Should().BeEquivalentTo("root");
            _configurations.Password.Should().BeEquivalentTo("secret");
        }
    }
}