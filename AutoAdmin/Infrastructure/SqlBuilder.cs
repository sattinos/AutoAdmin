﻿using System;
using System.Text;
using AutoAdmin.Core.Collections.Extensions;
using AutoAdmin.Model;

namespace AutoAdmin.Infrastructure {
    public class SqlBuilder<TKeyType, T> where T : BaseEntity<TKeyType> {
        private readonly BaseRepository<TKeyType, T> _repository;

        private readonly StringBuilder _stringBuilder = new (200);

        public SqlBuilder(BaseRepository<TKeyType, T> repository)
        {
            _repository = repository;
        }

        public SqlBuilder<TKeyType, T> Select(string[] columns = null) {
            // ReSharper disable once AssignNullToNotNullAttribute
            var finalColumns = EnumerableExtensions.IsNullOrEmpty(columns) ? "*" : String.Join(", ", columns);
            _stringBuilder.AppendLine($"Select {finalColumns} From {_repository.TableName}");
            return this;
        }

        public SqlBuilder<TKeyType, T> Delete()
        {
            _stringBuilder.AppendLine($"DELETE FROM {_repository.TableName}");
            return this;
        }

        public SqlBuilder<TKeyType, T> Where(string condition) {
            if (!string.IsNullOrWhiteSpace(condition)) {
                _stringBuilder.AppendLine($"where {condition} ");
            }
            return this;
        }

        public SqlBuilder<TKeyType, T> Page(int pageIndex, int pageSize) {
            _stringBuilder.AppendLine($"LIMIT {pageSize} OFFSET {pageSize * pageIndex};");
            return this;
        }

        public SqlBuilder<TKeyType, T> Count(string targetTable)
        {
            _stringBuilder.AppendLine($"SELECT COUNT(*) FROM {targetTable}");
            return this;
        }

        public SqlBuilder<TKeyType, T> InsertOne(T entity)
        {
            var first = true;
            var values = new StringBuilder();
            var columns = new StringBuilder();
            LastInsertedId = null;

            var excludedProperties = new [] {"IsIdAutoGenerated"}; 
            
            foreach (var property in _repository.Properties) {
                if (Array.IndexOf(excludedProperties, property.Name) != -1)
                {
                    continue;
                }
                var v = property.GetValue(entity);
                if( v != null ) {
                    if (!first) {
                        values.Append(",");
                        columns.Append(",");
                    }
                    var transformedProperty = PropertyTransformer.PropertyTransformer.Transform(property, entity);
                    if ( String.Equals(property.Name, "Id", StringComparison.OrdinalIgnoreCase) && !entity.IsIdAutoGenerated )
                    {
                        LastInsertedId = transformedProperty;
                    }
                    values.Append(transformedProperty);
                    columns.Append(property.Name);
                    first = false;
                }
            }

            _stringBuilder.AppendLine($"INSERT INTO {_repository.TableName} ({columns}) VALUES ({values})");
            return this;
        }
        
        public SqlBuilder<TKeyType, T> UpdateOne(T entity)
        {
            var first = true;
            var excludedProperties = new [] { "IsIdAutoGenerated", "Id" };
            
            _stringBuilder.Append($"Update {_repository.TableName} SET ");
            
            foreach (var property in _repository.Properties) {
                if (Array.IndexOf(excludedProperties, property.Name) != -1)
                {
                    continue;
                }
                var v = property.GetValue(entity);
                if( v != null ) {
                    if (!first) {
                        _stringBuilder.Append(",");
                    }
                    var transformedProperty = PropertyTransformer.PropertyTransformer.Transform(property, entity);
                    _stringBuilder.Append($"{property.Name} = {transformedProperty}");
                    first = false;
                }
            }
            _stringBuilder.Append(" ");
            Where($"Id={entity.Id}");
            return this;
        }

        public string LastInsertedId { get; private set; }
        public string Sql {
            get {
                return _stringBuilder.ToString();
            }
        }

        public void Reset() {
            _stringBuilder.Clear();
        }
    }
}
