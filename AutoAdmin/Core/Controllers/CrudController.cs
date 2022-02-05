using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using AutoAdmin.Core.Infrastructure;
using AutoAdmin.Core.Model;
using Microsoft.AspNetCore.Mvc;
using AutoAdmin.Dto;

namespace AutoAdmin.Core.Controllers
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
        private readonly BaseRepository<TKeyType, T> _baseRepository;

        public CrudController(BaseRepository<TKeyType, T> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        [HttpPost(Endpoints.GetOne)]
        public Task<T> GetOneAsync([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _baseRepository.GetOneAsync(queryDto?.Columns, queryDto?.Condition, parameters);
        }
        
        [HttpPost(Endpoints.GetMany)]
        public Task<IEnumerable<T>> GetManyAsync([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _baseRepository.GetManyAsync(queryDto?.Columns, queryDto?.Condition, parameters);
        }

        [HttpPost(Endpoints.Count)]
        public Task<int> CountAsync([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _baseRepository.CountAsync(queryDto?.Condition, parameters);
        }
        
        [HttpPost(Endpoints.InsertOne)]
        public Task<T> InsertOne([FromBody] T entity)
        {
            return _baseRepository.InsertOneAsync(entity);
        }
        
        [HttpPost(Endpoints.InsertMany)]
        public Task<int> InsertMany([FromBody] T[] entity)
        {
            return _baseRepository.InsertManyAsync(entity);
        }
        
        [HttpPost(Endpoints.UpdateOne)]
        public Task<int> UpdateOne([FromBody] UpdateQueryDto<T> queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _baseRepository.UpdateOneAsync(queryDto.Entity, queryDto.Columns, queryDto.Condition, parameters);
        }

        [HttpPost(Endpoints.UpdateMany)]
        public Task<int> UpdateMany([FromBody] UpdateQueryDto<T> queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _baseRepository.UpdateAsync(queryDto.Entity, queryDto.Columns, queryDto.Condition, parameters);
        }
        
        [HttpDelete("{Endpoints.DeleteOne}/{id}")]
        public Task<int> DeleteOne(TKeyType id)
        {
            return _baseRepository.DeleteAsync($"id={id}");
        }
        
        [HttpPost(Endpoints.DeleteMany)]
        public Task<int> DeleteMany([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _baseRepository.DeleteAsync(queryDto?.Condition, parameters);
        }
    }
}
