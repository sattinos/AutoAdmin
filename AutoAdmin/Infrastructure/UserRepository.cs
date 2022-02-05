using AutoAdmin.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using AutoAdmin.Injection.Attributes;
using AutoAdmin.Model;
using AutoAdmin.Services;

namespace AutoAdmin.Infrastructure {
    [InjectAs(ServiceLifetime.Singleton)]
    public class UserRepository : BaseRepository<uint, User> {
        public UserRepository(DbContext dbContext) : base(dbContext) {
        }
    }
}
