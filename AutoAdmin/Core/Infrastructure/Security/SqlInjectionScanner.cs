using System;
using System.Collections.Generic;
using AutoAdmin.Core.Collections.Extensions;
using AutoAdmin.Core.Text;

namespace AutoAdmin.Core.Infrastructure.Security
{
    public static class SqlInjectionScanner
    {
        private const string WhereConditionPattern = @"^\((?<columnName>[_a-zA-Z][_a-zA-Z0-9]*) (not )?(<|<=|>|>=|=|<>|like) (@p\d*)\)( ((AND)|(OR)) \(((?<columnName>[_a-zA-Z][_a-zA-Z0-9]*) (not )?(<|<=|>|>=|=|<>|like) (@p\d*))\))*";
        public static List<string> ScanCondition(string whereCondition, IEnumerable<string> entityColumns)
        {
            var columnsInWhereCondition = GetColumnsNames(whereCondition);
            ScanColumns(columnsInWhereCondition, entityColumns);
            return columnsInWhereCondition;
        }
        
        public static void ScanColumns(IEnumerable<string> columns, IEnumerable<string> entityColumns) {
            if (columns == null) {
                return;
            }
            if (!StringList.HasAll(entityColumns, columns, StringComparison.OrdinalIgnoreCase, out var notFoundElement)) {
                throw new Exception($"{notFoundElement} column are not found in the entity.");
            }
        }
        
        public static List<string> GetColumnsNames(string whereCondition)
        {
            return RegularExpressionsUtilities.GetAllCaptures(WhereConditionPattern, whereCondition, "columnName");
        }
    }
}