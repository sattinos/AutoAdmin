using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoAdminLib.Infrastructure.Security;
using AutoAdminLib.Model;
using AutoAdminLib.Services;
using Dapper;

namespace AutoAdminLib.Infrastructure {
    public abstract class CrudRepository<TKeyType, T> where T : BaseEntity<TKeyType> {
        private readonly DbContext _dbContext;
        private readonly SqlBuilder<TKeyType, T> _sqlBuilder;
        private readonly bool _isGuid = typeof(TKeyType) == typeof(Guid);

        protected CrudRepository(DbContext dbContext) {
            _dbContext = dbContext;
            TableName = typeof(T).Name;
            FetchEntityColumns();
            _sqlBuilder = new SqlBuilder<TKeyType, T>(this);
        }

        private void FetchEntityColumns() {
            var names = new List<string>();
            Properties = typeof(T).GetProperties();
            foreach (var column in Properties) {
                names.Add(column.Name);
            }
            EntityColumns = names.ToArray();
        }

        public string TableName { get; }
        private string[] EntityColumns { get; set; }
        public PropertyInfo[] Properties { get; set; }

        public Task<IEnumerable<T>> GetManyAsync(string[] columns = null,
                                     string condition = null,
                                     object parameters = null,
                                     int pageIndex = 0,
                                     int pageSize = 5) {
            return GetManyAsync<T>(columns, condition, parameters, pageIndex, pageSize);
        }
        private Task<IEnumerable<TU>> GetManyAsync<TU>(string[] columns = null,
            string condition = null,
            object parameters = null,
            int pageIndex = 0,
            int pageSize = 5) {
            SqlInjectionScanner.ScanColumns(columns, EntityColumns);
            _sqlBuilder.Reset();
            _sqlBuilder.Select(columns)
                .Where(condition)
                .Page(pageIndex, pageSize);
            return _dbContext.Connection.QueryAsync<TU>(_sqlBuilder.Sql, parameters);
        }

        public Task<IEnumerable<dynamic>> GetManyCompactAsync(string[] columns = null,
            string condition = null,
            object parameters = null,
            int pageIndex = 0,
            int pageSize = 5)
        {
            return GetManyAsync<dynamic>(columns, condition, parameters, pageIndex, pageSize);
        }

        public Task<T> GetOneAsync(string[] columns = null,
                                         string condition = null,
                                         object parameters = null)
        {
            return QueryFirstOrDefaultAsync<T>(columns, condition, parameters);
        }
        
        public Task<dynamic> GetOneCompactAsync(string[] columns = null,
            string condition = null,
            object parameters = null)
        {
            return QueryFirstOrDefaultAsync<dynamic>(columns, condition, parameters);
        }

        private Task<TU> QueryFirstOrDefaultAsync<TU>(string[] columns, string condition, object parameters)
        {
            SqlInjectionScanner.ScanColumns(columns, EntityColumns);
            SqlInjectionScanner.ScanCondition(condition, EntityColumns);

            _sqlBuilder.Reset();
            _sqlBuilder.Select(columns)
                .Where(condition);
            return _dbContext.Connection.QueryFirstOrDefaultAsync<TU>(_sqlBuilder.Sql, parameters);
        }

        public Task<T> GetByIdAsync(TKeyType idArg) {
            var where = _isGuid ? "Id = UUID_TO_BIN(@id)" : $"Id = @id";
            if (_isGuid) {
                return GetOneAsync(null, where, new { id = idArg.ToString() });
            }
            return GetOneAsync(null, where, new { id = idArg });
        }

        public async Task<T> InsertOneAsync(T entity) {
            _sqlBuilder.Reset();
            _sqlBuilder.InsertOne(entity);
            await _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql);

            _sqlBuilder.Reset();
            _sqlBuilder.Select()
                       .Where($"`id`= {_sqlBuilder.LastInsertedId ?? "LAST_INSERT_ID()" }");

            return await _dbContext.Connection.QueryFirstOrDefaultAsync<T>(_sqlBuilder.Sql);
        }
        
        public async Task<int> InsertManyAsync(IEnumerable<T> entity) {
            _sqlBuilder.Reset();
            _sqlBuilder.InsertMany(entity);
            var res = await _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql);
            return res;
        }

        public Task<int> UpdateOneAsync(T entity, string[] columns, string condition = null, object parameters = null)
        {
            SqlInjectionScanner.ScanColumns(columns, EntityColumns);
            SqlInjectionScanner.ScanCondition(condition, EntityColumns);
            _sqlBuilder.Reset();
            _sqlBuilder.UpdateOne(entity, columns);
            _sqlBuilder.AppendCondition(condition);
            return _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql, parameters);
        }
        
        public Task<int> UpdateAsync(T entity, string[] columns, string condition, object parameters = null)
        {
            SqlInjectionScanner.ScanColumns(columns, EntityColumns);
            SqlInjectionScanner.ScanCondition(condition, EntityColumns);
            _sqlBuilder.Reset();
            _sqlBuilder.Update(entity, columns);
            _sqlBuilder.Where(condition);
            return _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql, parameters);
        }

        public Task<int> CountAsync(
            string condition = null,
            object parameters = null)
        {
            SqlInjectionScanner.ScanCondition(condition, EntityColumns);
            _sqlBuilder.Reset();
            _sqlBuilder.Count(TableName);
            _sqlBuilder.Where(condition);
            return _dbContext.Connection.QueryFirstOrDefaultAsync<int>(_sqlBuilder.Sql, parameters);
        }

        public Task<int> DeleteAsync(string condition = null, object parameters = null)
        {
            SqlInjectionScanner.ScanCondition(condition, EntityColumns);
            _sqlBuilder.Reset();
            _sqlBuilder.Delete();
            if (!string.IsNullOrWhiteSpace(condition))
            {
                _sqlBuilder.Where(condition);
                return _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql, parameters);
            }
            return _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql);
        }
    }
}
