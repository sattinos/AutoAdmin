﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using AutoAdmin.Services;
using AutoAdmin.Core.Collections.Extensions;
using AutoAdmin.Model;

namespace AutoAdmin.Infrastructure {
    public abstract class BaseRepository<TKeyType, T> where T : BaseEntity<TKeyType> {
        private readonly DbContext _dbContext;
        private readonly SqlBuilder<TKeyType, T> _sqlBuilder;
        private readonly bool _isGuid = typeof(TKeyType) == typeof(Guid);

        public BaseRepository(DbContext dbContext) {
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
            Columns = names.ToArray();
        }

        public string TableName { get; }
        private string[] Columns { get; set; }
        public PropertyInfo[] Properties { get; set; }

        public Task<IEnumerable<T>> GetManyAsync(string[] columns = null,
                                     string condition = null,
                                     object parameters = null,
                                     int pageIndex = 0,
                                     int pageSize = 5) {
            ScanColumns(columns);
            _sqlBuilder.Reset();
            _sqlBuilder.Select(columns)
                       .Where(condition)
                       .Page(pageIndex, pageSize);
            return _dbContext.Connection.QueryAsync<T>(_sqlBuilder.Sql, parameters);
        }

        public Task<T> GetOneAsync(string[] columns = null,
                                         string condition = null,
                                         object parameters = null) {
            ScanColumns(columns);
            _sqlBuilder.Reset();
            _sqlBuilder.Select(columns)
                       .Where(condition);
            return _dbContext.Connection.QueryFirstOrDefaultAsync<T>(_sqlBuilder.Sql, parameters);
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
            _sqlBuilder.Reset();
            _sqlBuilder.UpdateOne(entity, columns);
            _sqlBuilder.AppendCondition(condition);
            return _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql, parameters);
        }
        
        // TODO: handle the case of value type properties, right now it only works for reference properties
        public Task<int> UpdateAsync(T entity, string[] columns, string condition, object parameters = null)
        {
            _sqlBuilder.Reset();
            _sqlBuilder.Update(entity, columns);
            _sqlBuilder.Where(condition);
            return _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql, parameters);
        }

        public Task<int> CountAsync(
            string condition = null,
            object parameters = null)
        {
            _sqlBuilder.Reset();
            _sqlBuilder.Count(TableName);
            _sqlBuilder.Where(condition);
            return _dbContext.Connection.QueryFirstOrDefaultAsync<int>(_sqlBuilder.Sql, parameters);
        }

        public Task<int> DeleteAsync(string condition = null, object parameters = null)
        {
            _sqlBuilder.Reset();
            _sqlBuilder.Delete();
            if (!string.IsNullOrWhiteSpace(condition))
            {
                _sqlBuilder.Where(condition);
                return _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql, parameters);
            }
            return _dbContext.Connection.ExecuteAsync(_sqlBuilder.Sql);
        }
        
        private void ScanColumns(IEnumerable<string> columns) {
            if (columns == null) {
                return;
            }
            if (!StringList.HasAll(Columns, columns, StringComparison.OrdinalIgnoreCase)) {
                throw new Exception("Requested columns are not found in the entity.");
            }
        }

        private void CalculateColumns(T entity)
        {
            var first = true;
            var columns = new StringBuilder();
            var excludedProperties = new string[] {"IsIdAutoGenerated"}; 
            
            foreach (var property in Properties) {
                if (Array.IndexOf(excludedProperties, property.Name) != -1)
                {
                    continue;
                }
                var v = property.GetValue(entity);
                if( v != null ) {
                    if (!first) {
                        columns.Append(",");
                    }
                    columns.Append(property.Name);
                    first = false;
                }
            }
        }
    }
}
