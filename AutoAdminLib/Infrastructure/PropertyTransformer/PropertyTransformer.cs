using System;
using System.Collections.Generic;
using System.Reflection;
using AutoAdminLib.Infrastructure.PropertyTransformer.MySql;

namespace AutoAdminLib.Infrastructure.PropertyTransformer
{
    public static class PropertyTransformer
    {
        private static readonly Dictionary<Type, IPropertyTransformer> Transformers = new Dictionary<Type, IPropertyTransformer>()
        {
            [typeof(Guid)] = new GuidTransformer(),
            [typeof(DateTime?)] = new NullableDateTimeTransformer(),
            [typeof(bool)] = new BooleanTransformer()
        };

        public static string Transform(PropertyInfo propertyInfo, object obj)
        {
            var propertyValue = propertyInfo.GetValue(obj);
            if (Transformers.ContainsKey(propertyInfo.PropertyType))
            {
                return Transformers[propertyInfo.PropertyType].ForwardTransformValue(propertyInfo, propertyValue);
            }
            return $"'{propertyValue}'"; 
        }
    }
}