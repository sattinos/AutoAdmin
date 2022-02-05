using System;
using AutoAdmin.Core.Dapper.MySql.TypeHandlers;
using Dapper;

namespace AutoAdmin.Services.Dapper {
    public static class GuidMapper {
        public static void Setup() {
            SqlMapper.ResetTypeHandlers();
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            SqlMapper.AddTypeHandler(typeof(Guid), GuidTypeHandler.Default);
        }
    }
}
