using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Extensions.Ordering;
using FluentAssertions;
using AutoAdmin.Infrastructure;
using AutoAdmin.IntegrationTest.Setup;
using AutoAdmin.Model;

namespace AutoAdmin.IntegrationTest.Infrastructure {
    [Collection("AutoAdmin Collection"), Order(3)]
    public class UserRepositoryTest {
        private readonly UserRepository _userRep;
        private static User _insertedUser = null;

        public const int NumberOfUsersToSeed = 1000;

        public UserRepositoryTest(TestFactory testFactory) {
            _userRep = testFactory.Server.Services.GetService<UserRepository>();
        }

        [Fact(DisplayName = "Get the table name based on entity class name"), Order(1)]
        public void GetTableNameTest() {
            var tableName = _userRep.TableName;
            tableName.Should().NotBeNull();
            tableName.Should().Be("User");
        }

        [Fact(DisplayName = "Should return all users"), Order(2)]
        public async Task GetAllTest() {
            var allUsers = (await _userRep.GetManyAsync(null, "Id < @id", new {id = 4})).ToList();
            allUsers.Should().NotBeNull();
            allUsers.Count.Should().BeGreaterThan(0);
            allUsers.Count.Should().BeLessThan(6);
        }

        [Fact(DisplayName = "Should return all users but each user has (Id, FullName, UserName) only"), Order(3)]
        public async Task GetAllColumnsSelectorTest() {

            var allUsers = (await _userRep.GetManyAsync(
                new [] { "Id", "FullName", "UserName" }, 
                "Id < @id", 
                new { id = 4 })).ToList();
            allUsers.Should().NotBeNull();
            allUsers.Count.Should().BeGreaterThan(0);
            foreach (var user in allUsers) {
                user.BirthDate.Should().BeNull();
                user.Phone.Should().BeNull();
                user.FullName.Should().NotBeNullOrEmpty();
                user.UserName.Should().NotBeNullOrEmpty();
            }
            allUsers.Count.Should().BeLessThan(6);
        }

        [Fact(DisplayName = "Should return all users but each user has Id only"), Order(4)]
        public async Task GetAllColumnsSelectorTestWithDateQuery() {
            var allUsers = (await _userRep.GetManyAsync(
                new [] { "Id" }, 
                "BirthDate < @birthDate", 
                new { birthDate = new DateTime(1995, 1, 1) })).ToList();
            allUsers.Should().NotBeNull();
            allUsers.Count.Should().BeGreaterThan(0);
            foreach (var user in allUsers) {
                user.Id.Should().BePositive();
                user.BirthDate.Should().BeNull();
                user.Phone.Should().BeNull();
                user.FullName.Should().BeNull();
                user.UserName.Should().BeNull();
            }
            allUsers.Count.Should().BeLessThan(6);
        }

        [Fact(DisplayName = "Should return single user with projected columns"), Order(5)]
        public async Task GetOneUserProjectedColumns() {
            var user = await _userRep.GetOneAsync(
                new [] {nameof(User.Id), "FullName", "UserName", "BirthDate"},
                "BirthDate < @birthDate", 
                new {birthDate = new DateTime(1995, 1, 1)});
            user.Should().NotBeNull();
            user.Id.Should().BeGreaterOrEqualTo(0);
            user.BirthDate.Should().NotBeNull();            
            user.FullName.Should().NotBeNullOrEmpty();
            user.UserName.Should().NotBeNullOrEmpty();
            user.Phone.Should().BeNull();
        }

        [Fact(DisplayName = "Should return single user with all columns"), Order(6)]
        public async Task GetOneUser() {
            var user = await _userRep.GetOneAsync(null, "BirthDate < @birthDate", new { birthDate = new DateTime(1995, 1, 1) });
            user.Should().NotBeNull();
            user.Id.Should().BeGreaterOrEqualTo(0);
            user.CreatedAt.Should().NotBeNull();
            user.CreatedBy.Should().NotBeNull();
            
            user.FullName.Should().NotBeNullOrEmpty();
            user.UserName.Should().NotBeNullOrEmpty();
            user.BirthDate.Should().NotBeNull();
            user.PasswordHash.Should().NotBeNull();
            user.PasswordSalt.Should().NotBeNull();
            user.Phone.Should().NotBeNullOrEmpty();
            user.Email.Should().NotBeNull();
            user.IsVerified.Should().BeTrue();
        }

        [Fact(DisplayName = "Should return single user by its id"), Order(7)]
        public async Task GetById() {
            var user = await _userRep.GetByIdAsync(1);
            user.Should().NotBeNull();
            user.Id.Should().BeGreaterOrEqualTo(1);
            user.CreatedAt.Should().NotBeNull();
            user.CreatedBy.Should().NotBeNull();

            user.FullName.Should().NotBeNullOrEmpty();
            user.UserName.Should().NotBeNullOrEmpty();
            user.BirthDate.Should().NotBeNull();
            user.PasswordHash.Should().NotBeNull();
            user.PasswordSalt.Should().NotBeNull();
            user.Phone.Should().NotBeNullOrEmpty();
            user.Email.Should().NotBeNull();
            user.IsVerified.Should().BeTrue();
        }
        
        [Fact(DisplayName = "Should insert single user successfully"), Order(8)]
        public async Task InsertOneTest()
        {
            var user = new User()
            {
                CreatedAt = DateTime.Now.Date,
                CreatedBy = "satinos",
                UpdatedAt = null,
                UpdatedBy = null,
                
                FullName = "Mahmoud AlSati",
                UserName = "satinos",
                BirthDate = DateTime.Now.Date,
                PasswordHash = "",
                PasswordSalt = "",
                Phone = "9844721555",
                Email = "someemail@gmail.com",
                IsVerified = false
            };
            var insertedUser = await _userRep.InsertOneAsync(user);
            insertedUser.Should().NotBeNull();
            insertedUser.Id.Should().BeGreaterThan(0);
            insertedUser.CreatedAt.Should().Be(user.CreatedAt);
            insertedUser.FullName.Should().Be(user.FullName);
            insertedUser.UserName.Should().Be(user.UserName);
            insertedUser.BirthDate.Should().Be(user.BirthDate);
            insertedUser.Phone.Should().Be(user.Phone);
            insertedUser.Email.Should().Be(user.Email);
            insertedUser.IsVerified.Should().Be(user.IsVerified);
            _insertedUser = insertedUser;
        }

        [Fact(DisplayName = "Should insert many users successfully"), Order(9)]
        public async Task InsertManyTest()
        {
            var users = new List<User>();
            for (int i = 0; i < NumberOfUsersToSeed; i++)
            {
                users.Add(new User()
                {
                    CreatedAt = DateTime.Now.Date,
                    CreatedBy = $"new-user{i}",
                    UpdatedAt = null,
                    UpdatedBy = null,
                
                    FullName = $"new-user{i}",
                    UserName = $"new-user{i}",
                    BirthDate = DateTime.Now.Date,
                    PasswordHash = "",
                    PasswordSalt = "",
                    Phone = $"984472155{i}",
                    Email = $"new-user{i}@gmail.com",
                    IsVerified = false
                });
            }
            var inserted = await _userRep.InsertManyAsync(users);
            inserted.Should().Be(users.Count);
        }
        
        [Fact(DisplayName = "Should update one successfully"), Order(10)]
        public async Task UpdateOneTest()
        {
            _insertedUser.Email = "newEmail1@gmail.com";
            _insertedUser.IsVerified = true;
            var affectedRows= await _userRep.UpdateOneAsync(_insertedUser, new []{ nameof(User.Email), nameof(User.IsVerified) });
            affectedRows.Should().Be(1);

            var user = await _userRep.GetByIdAsync(_insertedUser.Id);
            user.Should().BeEquivalentTo(_insertedUser);
            user.IsVerified.Should().Be(true);
        }
        
        [Fact(DisplayName = "Should update many users successfully"), Order(11)]
        public async Task UpdateManyTest()
        {
            _insertedUser.Email = "newEmail1@gmail.com";
            _insertedUser.IsVerified = true;

            var whereCondition = "UserName LIKE @p";
            var parameters = new {p = "%new-user%"};
            var affectedRows= await _userRep.UpdateAsync(new User() { PasswordHash = "$2a$12$bz1EsslS/F4WXSiFqGepaenTCCawZjmdYrxcEaVnSLPNKEkhQdWgK" },
                new []{ nameof(User.PasswordHash)},
                whereCondition, parameters);
            affectedRows.Should().Be(NumberOfUsersToSeed);
        }
        
        [Fact(DisplayName = "Should delete satinos user"), Order(12)]
        public async Task DeleteTest()
        {
            var affectedRows= await _userRep.DeleteAsync("UserName = @un", new { un = "satinos" });
            affectedRows.Should().Be(1);
        }
        
        [Fact(DisplayName = "Should count all users"), Order(13)]
        public async Task CountAllTest()
        {
            var count= await _userRep.CountAsync();
            count.Should().Be(NumberOfUsersToSeed+8);
        }

        [Fact(DisplayName = "Should count all users that satisfy where condition"), Order(14)]
        public async Task CountWhereTest()
        {
            var whereCondition = "UserName LIKE @p";
            var parameters = new {p = "%new-user%"};
            var count= await _userRep.CountAsync(whereCondition, parameters);
            count.Should().Be(NumberOfUsersToSeed);
        }
    }
}
