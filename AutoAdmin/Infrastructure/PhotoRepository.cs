using System;
using Microsoft.Extensions.DependencyInjection;
using AutoAdmin.Core.Infrastructure;
using AutoAdmin.Injection.Attributes;
using AutoAdmin.Model;
using AutoAdmin.Services;

namespace AutoAdmin.Infrastructure {
    [InjectAs(ServiceLifetime.Singleton)]
    public class PhotoRepository : BaseRepository<Guid, Photo> {
        public PhotoRepository(DbContext dbContext) : base(dbContext) {
        }
    }
}
