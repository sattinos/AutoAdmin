using System;
using Microsoft.Extensions.DependencyInjection;
using AutoAdminLib.Infrastructure;
using AutoAdminLib.Injection.Attributes;
using AutoAdminLib.Services;
using AutoAdminLib.TestWebApp.Model;

namespace AutoAdminLib.TestWebApp.Infrastructure {
    [InjectAs(ServiceLifetime.Singleton)]
    public class PhotoRepository : BaseRepository<Guid, Photo> {
        public PhotoRepository(DbContext dbContext) : base(dbContext) {
        }
    }
}
