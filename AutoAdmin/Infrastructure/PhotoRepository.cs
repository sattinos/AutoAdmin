using System;
using AutoAdmin.Injection.Attributes;
using AutoAdmin.Model;
using AutoAdmin.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AutoAdmin.Infrastructure {
    [InjectAs(ServiceLifetime.Singleton)]
    public class PhotoRepository : BaseRepository<Guid, Photo> {
        public PhotoRepository(DbContext dbContext) : base(dbContext) {
        }
    }
}
