using System;
using System.Reflection;

namespace AutoAdmin.Core.Infrastructure.PropertyTransformer.MySql
{
    public class NullableDateTimeTransformer : IPropertyTransformer
    {
        public string Mask { get; set; } = "yyyy-MM-dd";
        
        public string ForwardTransformValue(PropertyInfo propertyInfo, object propertyValue)
        {
            var dateTimeValue = ((DateTime?) propertyValue).Value;
            return $"'{dateTimeValue.ToString(Mask)}'";
        }
    }
}