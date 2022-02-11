using Microsoft.Extensions.DependencyInjection;
using AutoAdminLib.Infrastructure;
using AutoAdminLib.Injection.Attributes;
using AutoAdminLib.Services;
using AutoAdminLib.TestWebApp.Model;

namespace AutoAdminLib.TestWebApp.Infrastructure {
    [InjectAs(ServiceLifetime.Singleton)]
    public class UserRepository : BaseRepository<uint, User> {
        public UserRepository(DbContext dbContext) : base(dbContext) {
        }
    }
}
