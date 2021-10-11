using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AutoAdmin.Infrastructure;
using AutoAdmin.Model;

namespace AutoAdmin.Controllers
{
    public readonly struct Endpoints
    {
        public const string GetOne = "getOne";
        public const string GetMany = "getMany";
        public const string Count = "count";
        public const string InsertOne = "insertOne";
        public const string InsertMany = "insertMany";
        public const string UpdateOne = "updateOne";
        public const string UpdateMany = "updateMany";
        public const string DeleteOne = "deleteOne";
        public const string DeleteMany = "deleteMany";
    }
    
    [Route("api/[controller]")]
    public class CrudController<TKeyType, T> : Controller where T : BaseEntity<TKeyType>
    {
        private BaseRepository<TKeyType, T> _baseRepository;

        public CrudController(BaseRepository<TKeyType, T> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        [HttpPost(Endpoints.GetOne)]
        public Task<T> GetOneAsync([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = null;
            if (queryDto != null && queryDto.Parameters != null)
            {
                parameters = JsonConvert.DeserializeObject<ExpandoObject>(queryDto?.Parameters);
            }
            return _baseRepository.GetOneAsync(queryDto?.Columns, queryDto?.Condition, parameters);
        }
        
        [HttpPost(Endpoints.GetMany)]
        public Task<IEnumerable<T>> GetManyAsync([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = null;
            if (queryDto != null && queryDto.Parameters != null)
            {
                parameters = JsonConvert.DeserializeObject<ExpandoObject>(queryDto?.Parameters);
            }
            return _baseRepository.GetMany(queryDto?.Columns, queryDto?.Condition, parameters);
        }

        [HttpPost(Endpoints.Count)]
        public Task<int> CountAsync([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = null;
            if (queryDto != null && queryDto.Parameters != null)
            {
                parameters = JsonConvert.DeserializeObject<ExpandoObject>(queryDto?.Parameters);
            }
            return _baseRepository.CountAsync(queryDto?.Condition, parameters);
        }
        
        [HttpPost(Endpoints.InsertOne)]
        public Task<T> InsertOne([FromBody] T entity)
        {
            return _baseRepository.InsertOneAsync(entity);
        }
        
        [HttpDelete("{Endpoints.DeleteOne}/{id}")]
        public Task<int> DeleteOne(TKeyType id)
        {
            return _baseRepository.DeleteAsync($"id={id}");
        }
        
        [HttpPost(Endpoints.DeleteMany)]
        public Task<int> DeleteMany([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = null;
            if (queryDto != null && queryDto.Parameters != null)
            {
                parameters = JsonConvert.DeserializeObject<ExpandoObject>(queryDto?.Parameters);
            }
            return _baseRepository.DeleteAsync(queryDto?.Condition, parameters);
        }
    }
}
