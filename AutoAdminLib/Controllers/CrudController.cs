using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoAdminLib.Dto;
using AutoAdminLib.Infrastructure;
using AutoAdminLib.Model;

namespace AutoAdminLib.Controllers
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
    public class CrudController<TKeyType, T> : ControllerBase where T : BaseEntity<TKeyType>
    {
        private readonly CrudRepository<TKeyType, T> _crudRepository;

        public CrudController(CrudRepository<TKeyType, T> crudRepository)
        {
            _crudRepository = crudRepository;
        }

        [HttpPost(Endpoints.GetOne)]
        public Task<dynamic> GetOneAsync([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _crudRepository.GetOneCompactAsync(queryDto?.Columns, queryDto?.Condition, parameters);
        }
        
        [HttpPost(Endpoints.GetMany)]
        public Task<IEnumerable<dynamic>> GetManyAsync([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _crudRepository.GetManyCompactAsync(queryDto?.Columns, queryDto?.Condition, parameters);
        }

        [HttpPost(Endpoints.Count)]
        public Task<int> CountAsync([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _crudRepository.CountAsync(queryDto?.Condition, parameters);
        }
        
        [HttpPost(Endpoints.InsertOne)]
        public Task<T> InsertOne([FromBody] T entity)
        {
            return _crudRepository.InsertOneAsync(entity);
        }
        
        [HttpPost(Endpoints.InsertMany)]
        public Task<int> InsertMany([FromBody] T[] entity)
        {
            return _crudRepository.InsertManyAsync(entity);
        }
        
        [HttpPost(Endpoints.UpdateOne)]
        public Task<int> UpdateOne([FromBody] UpdateQueryDto<T> queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _crudRepository.UpdateOneAsync(queryDto.Entity, queryDto.Columns, queryDto.Condition, parameters);
        }

        [HttpPost(Endpoints.UpdateMany)]
        public Task<int> UpdateMany([FromBody] UpdateQueryDto<T> queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _crudRepository.UpdateAsync(queryDto.Entity, queryDto.Columns, queryDto.Condition, parameters);
        }
        
        [HttpDelete("{Endpoints.DeleteOne}/{id}")]
        public Task<int> DeleteOne(TKeyType id)
        {
            return _crudRepository.DeleteAsync($"id={id}");
        }
        
        [HttpPost(Endpoints.DeleteMany)]
        public Task<int> DeleteMany([FromBody] QueryDto queryDto)
        {
            ExpandoObject parameters = queryDto?.ConvertParametersToExpando();
            return _crudRepository.DeleteAsync(queryDto?.Condition, parameters);
        }
    }
}
