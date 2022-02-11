using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Extensions.Ordering;
using FluentAssertions;
using AutoAdminLib.TestWebApp.Infrastructure;
using AutoAdminLib.TestWebApp.Model;
using AutoAdminLib.IntegrationTest.Setup;

namespace AutoAdminLib.IntegrationTest.Infrastructure {
    [Collection("AutoAdmin Collection"), Order(4)]
    public class PhotoRepositoryTest {
        private readonly PhotoRepository _photoRep;

        public PhotoRepositoryTest(TestFactory testFactory) {
            _photoRep = testFactory.Server.Services.GetService<PhotoRepository>();
        }

        [Fact(DisplayName = "Should return single photo by its id")]
        public async Task GetById() {
           
            var id = new Guid("a8a7cefb-651d-11eb-837c-7cd30a813cc1");
            var photo = await _photoRep.GetByIdAsync(id);
            photo.Should().NotBeNull();
            photo.Id.Should().Be(id);
            photo.CreatedAt.Should().NotBeNull();
            photo.CreatedBy.Should().NotBeNull();
            photo.FileId.Should().NotBeEmpty();
        }
        
        [Fact(DisplayName = "Should return single photo with all columns")]
        public async Task GetOnePhoto() {            
            var id = new Guid("a8a7d033-651d-11eb-837c-7cd30a813cc1");
            var photo = await _photoRep.GetOneAsync(null, "Name = @name", new { name = "arizona" });
            photo.Should().NotBeNull();
            photo.Id.Should().Be(id);
        }
        
        [Fact(DisplayName = "Should return single photo with selected columns only")]
        public async Task GetOnePhotoWithProjection() {            
            var id = new Guid("a8a7d033-651d-11eb-837c-7cd30a813cc1");
            var photo = await _photoRep.GetOneAsync( new []{nameof(Photo.Name), nameof(Photo.Id)}, "Name = @name", new { name = "arizona" });
            photo.Should().NotBeNull();
            photo.Id.Should().Be(id);
            photo.FileId.Should().BeEmpty();
            photo.Name.Should().NotBeNullOrWhiteSpace();
            photo.CreatedAt.Should().BeNull();
        }
        
        [Fact(DisplayName = "Should refuse not found selected columns")]
        public async Task GetOnePhotoWithProjectionShouldRefuse()
        {
            Exception ex = null;
            try
            {
                var id = new Guid("a8a7d033-651d-11eb-837c-7cd30a813cc1");
                var photo = await _photoRep.GetOneAsync( new []{nameof(Photo.Name), nameof(Photo.Id), "someHackedName"}, "Name = @name", new { name = "arizona" });
                photo.Should().NotBeNull();
                photo.Id.Should().Be(id);
                photo.FileId.Should().BeEmpty();
                photo.Name.Should().NotBeNullOrWhiteSpace();
                photo.CreatedAt.Should().BeNull();
            }
            catch (Exception e)
            {
                ex = e;
            }

            ex.Should().NotBeNull();
            ex.Message.Should().Be("someHackedName column are not found in the entity.");
        }
        
        [Fact(DisplayName = "Should insert single photo successfully")]
        public async Task InsertOneTest()
        {
            var photo = new Photo
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.Date,
                CreatedBy = "satinos",
                UpdatedAt = null,
                UpdatedBy = null,
                Name = "Profile Photo",
                FileId = Guid.NewGuid()
            };
            var insertedPhoto = await _photoRep.InsertOneAsync(photo);
            insertedPhoto.Should().NotBeNull();
            insertedPhoto.Id.Should().Be(photo.Id);
            insertedPhoto.CreatedAt.Should().Be(photo.CreatedAt);
            insertedPhoto.Name.Should().Be(photo.Name);
            insertedPhoto.FileId.Should().Be(photo.FileId);
        }
    }
}
