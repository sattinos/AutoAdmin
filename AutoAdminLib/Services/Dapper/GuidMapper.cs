using System;
using Dapper;
using AutoAdminLib.Dapper.MySql.TypeHandlers;

namespace AutoAdminLib.Services.Dapper {
    public static class GuidMapper {
        public static void Setup() {
            SqlMapper.ResetTypeHandlers();
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            SqlMapper.AddTypeHandler(typeof(Guid), GuidTypeHandler.Default);
        }
    }
}
