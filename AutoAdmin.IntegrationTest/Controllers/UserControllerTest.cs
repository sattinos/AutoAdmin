using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoAdmin.Core.Controllers;
using AutoAdmin.Dto;
using AutoAdmin.IntegrationTest.Infrastructure;
using AutoAdmin.IntegrationTest.Setup;
using AutoAdmin.Model;
using Xunit;
using Xunit.Extensions.Ordering;
using Newtonsoft.Json;
using FluentAssertions;

namespace AutoAdmin.IntegrationTest.Controllers
{
    [Collection("AutoAdmin Collection"), Order(5)]
    public class UserControllerTest
    {
        private readonly HttpClient _httpClient;
        private static User InsertedUser = null;

        public UserControllerTest(TestFactory testFactory)
        {
            _httpClient = testFactory.CreateClient();
        }

        [Fact(DisplayName = "Should return one user with all columns")]
        public async Task GetOneAsyncAllColumnsTest()
        {
            var res = await _httpClient.PostAsJsonAsync<QueryDto>("api/User/getOne", null);
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await res.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);
            user.Should().NotBeNull();
        }

        [Fact(DisplayName = "Should return one user with only selected columns")]
        public async Task GetOneAsyncSelectedColumnsTest()
        {
            var user = await FetchUserById(2);
            user.Should().NotBeNull();
            user.Email.Should().NotBeNullOrWhiteSpace();
            user.Id.Should().BeGreaterOrEqualTo(0);
            user.Phone.Should().BeNull();
        }

        private async Task<User> FetchUserById(uint id)
        {
            var res = await _httpClient.PostAsJsonAsync("api/User/getOne",
                new QueryDto()
                {
                    Columns = new[] { nameof(User.Id), nameof(User.Email), nameof(User.FullName) }, 
                    Condition = "Id=@id",
                    Parameters = JsonConvert.SerializeObject(new { id })
                });
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<User>(content);
        }

        [Fact(DisplayName = "Should return all users")]
        public async Task GetAllTest()
        {
            var res = await _httpClient.PostAsJsonAsync<QueryDto>($"api/User/{Endpoints.GetMany}", null);
            var content = await res.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrWhiteSpace();
            var users = JsonConvert.DeserializeObject<User[]>(content);
            users.Should().NotBeNull();
            users.Length.Should().BePositive();
        }
        
        [Fact(DisplayName = "Should return all users with only selected columns")]
        public async Task GetAllSelectedColumnsTest()
        {
            var res = await _httpClient.PostAsJsonAsync<QueryDto>($"api/User/{Endpoints.GetMany}", 
                new QueryDto()
                {
                    Columns = new[] { nameof(User.Id), nameof(User.Email) }
                });
            var content = await res.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrWhiteSpace();
            var users = JsonConvert.DeserializeObject<User[]>(content);
            users.Should().NotBeNull();
            users.Length.Should().BePositive();
            foreach (var user in users)
            {
                user.Should().NotBeNull();
                user.Email.Should().NotBeNullOrWhiteSpace();
                user.Id.Should().BeGreaterOrEqualTo(0);
                user.Phone.Should().BeNull();
                user.BirthDate.Should().BeNull();
                user.FullName.Should().BeNull();
            }
        }
        
        [Fact(DisplayName = "Should return all users with only selected columns that satisfy a specific condition")]
        public async Task GetAllWhereTest()
        {
            var res = await _httpClient.PostAsJsonAsync<QueryDto>($"api/User/{Endpoints.GetMany}", 
                new QueryDto()
                {
                    Columns = new[] { nameof(User.Id), nameof(User.Email) },
                    Condition = "Id>@id",
                    Parameters = JsonConvert.SerializeObject(new { id = 6 })
                });
            var content = await res.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrWhiteSpace();
            var users = JsonConvert.DeserializeObject<User[]>(content);
            users.Should().NotBeNull();
            users.Length.Should().Be(5);
            foreach (var user in users)
            {
                user.Should().NotBeNull();
                user.Email.Should().NotBeNullOrWhiteSpace();
                user.Id.Should().BeGreaterOrEqualTo(0);
                user.Phone.Should().BeNull();
                user.BirthDate.Should().BeNull();
                user.FullName.Should().BeNull();
            }
        }

        [Fact(DisplayName = "Should count all users without constraints")]
        public async Task CountAllTest()
        {
            var res = await _httpClient.PostAsJsonAsync<QueryDto>($"api/User/{Endpoints.Count}", null);
            var count = int.Parse(await res.Content.ReadAsStringAsync());
            count.Should().Be(UserRepositoryTest.NumberOfUsersToSeed + 8);
        }
        
        [Fact(DisplayName = "Should count users with constraints")]
        public async Task CountAllWhereTest()
        {
            var res = await _httpClient.PostAsJsonAsync<QueryDto>($"api/User/{Endpoints.Count}", 
                new QueryDto()
                {
                    Condition = "IsVerified=@p",
                    Parameters = JsonConvert.SerializeObject(new { p = true} )
                });
            var count = int.Parse(await res.Content.ReadAsStringAsync());
            count.Should().Be(5);
        }
        
        [Fact(DisplayName = "Should insert one user successfully")]
        public async Task InsertOneTest()
        {
            var res = await _httpClient.PostAsJsonAsync<User>($"api/User/{Endpoints.InsertOne}", 
                    new User()
                    {
                        CreatedAt = DateTime.Now.Date,
                        CreatedBy = "aous",
                        UpdatedAt = null,
                        UpdatedBy = null,
                
                        FullName = "Aous AlSati",
                        UserName = "aous",
                        BirthDate = DateTime.Now.Date,
                        PasswordHash = "",
                        PasswordSalt = "",
                        Phone = "9744711515",
                        Email = "someemaile@gmail.com",
                        IsVerified = false
                    }
                );
            InsertedUser = JsonConvert.DeserializeObject<User>(await res.Content.ReadAsStringAsync());
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            InsertedUser.Should().NotBeNull();
        }
        
        [Fact(DisplayName = "Should insert many users successfully")]
        public async Task InsertManyUsersTest()
        {
            var users = new List<User>();
            const int count = 100;
            const int offset = 0;
            for (int i = offset; i < count + offset; i++)
            {
                users.Add(new User()
                {
                    CreatedAt = DateTime.Now.Date,
                    CreatedBy = $"api-user{i}",
                    UpdatedAt = null,
                    UpdatedBy = null,
                
                    FullName = $"api-user{i}",
                    UserName = $"api-user{i}",
                    BirthDate = DateTime.Now.Date,
                    PasswordHash = "",
                    PasswordSalt = "",
                    Phone = $"984472155{i}",
                    Email = $"api-user{i}@gmail.com",
                    IsVerified = true
                });
            }
            
            var res = await _httpClient.PostAsJsonAsync<List<User>>($"api/User/{Endpoints.InsertMany}", users);
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var insertedCount = int.Parse(await res.Content.ReadAsStringAsync());
            insertedCount.Should().Be(users.Count);
        }

        [Fact(DisplayName = "Should update one user successfully")]
        public async Task UpdateOneTest()
        {
            var discardedBirthDate = DateTime.Now.Date.AddYears(-10);
            var res = await _httpClient.PostAsJsonAsync<UpdateQueryDto<User>>($"api/User/{Endpoints.UpdateOne}",
                new UpdateQueryDto<User>()
                {
                    Entity = new User()
                    {
                        Id = InsertedUser.Id,
                        FullName = "Modified from API",
                        BirthDate = discardedBirthDate
                    },
                    Columns = new[] { nameof(User.FullName) } 
                });
            res.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseAsString = await res.Content.ReadAsStringAsync();
            var numberOfUpdated = int.Parse(responseAsString);
            numberOfUpdated.Should().Be(1);
            
            var updatedUser = await FetchUserById(InsertedUser.Id);
            updatedUser.FullName.Should().Be("Modified from API");
            updatedUser.BirthDate.Should().NotBe(discardedBirthDate);
        }
        
        [Fact(DisplayName = "Should update many users successfully")]
        public async Task UpdateManyTest()
        {
            var countApiCallResult = await _httpClient.PostAsJsonAsync<QueryDto>($"api/User/{Endpoints.Count}", 
                new QueryDto()
                {
                    Condition = "IsVerified=@p",
                    Parameters = JsonConvert.SerializeObject(new { p = true} )
                });
            var count = int.Parse(await countApiCallResult.Content.ReadAsStringAsync());
            
            var targetHash = "$4v$14$bz1HsUl/B4WXSiFqGepaenTCCawZjmdYrxcEaVnSLPNKEkhQdTgM";
            var res = await _httpClient.PostAsJsonAsync<UpdateQueryDto<User>>($"api/User/{Endpoints.UpdateMany}",
                new UpdateQueryDto<User>()
                {
                    Entity = new User()
                    {
                        PasswordHash = targetHash
                    },
                    Columns = new[] { nameof(User.PasswordHash) },
                    Condition = "IsVerified=@p",
                    Parameters = JsonConvert.SerializeObject(new { p = true })
                });
            res.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseAsString = await res.Content.ReadAsStringAsync();
            var numberOfUpdated = int.Parse(responseAsString);
            numberOfUpdated.Should().Be(count); 
        }

        [Fact(DisplayName = "Delete One by Id")]
        public async Task DeleteOneByIdTest()
        {
            var res = await _httpClient.DeleteAsync($"api/User/{Endpoints.DeleteOne}/5");
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var deletedEntitiesCount = int.Parse(await res.Content.ReadAsStringAsync());
            deletedEntitiesCount.Should().Be(1);
        }

        [Fact(DisplayName = "Delete Many with where condition: Should delete all unverified users")]
        public async Task DeleteManyWhereTest()
        {
            var res = await _httpClient.PostAsJsonAsync($"api/User/{Endpoints.DeleteMany}", new QueryDto()
            {
                Condition = "IsVerified=@p",
                Parameters = JsonConvert.SerializeObject(new { p = 0 }) 
            });
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var deletedEntitiesCount = int.Parse(await res.Content.ReadAsStringAsync());
            deletedEntitiesCount.Should().Be(UserRepositoryTest.NumberOfUsersToSeed + 4);
        }
    }
}