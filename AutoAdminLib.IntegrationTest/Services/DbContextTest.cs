using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Extensions.Ordering;
using FluentAssertions;
using AutoAdminLib.Services;
using AutoAdminLib.IntegrationTest.Setup;

namespace AutoAdminLib.IntegrationTest.Services {
    [Collection("AutoAdmin Collection"), Order(2)]
    public class DbContextTest {
        private readonly DbContext _dbContext;
        public DbContextTest(TestFactory testFactory)
        {
            testFactory.CreateClient();
            _dbContext = testFactory.Server.Services.GetService<DbContext>();
        }

        [Fact(DisplayName = "database connectivity test"), Order(1)]
        public void ShouldConnectSuccessfully() {
            _dbContext.Should().NotBeNull();
            _dbContext.Connection.State.Should().Be(ConnectionState.Closed);
        }
    }
}
