using System;
using System.Data;
using Dapper;

namespace AutoAdminLib.Dapper.MySql.TypeHandlers {
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid> {

        public static readonly SqlMapper.ITypeHandler Default = new GuidTypeHandler();

        public override Guid Parse(object value) {
            
            Array.Reverse((byte[])value, 0, 4); 
            Array.Reverse((byte[])value, 4, 2); 
            Array.Reverse((byte[])value, 6, 2);
            
            return new Guid((byte[])value);
        }

        public override void SetValue(IDbDataParameter parameter, Guid value) {
            parameter.DbType = DbType.Guid;
            parameter.Value = value.ToByteArray();
        }
    }
}
