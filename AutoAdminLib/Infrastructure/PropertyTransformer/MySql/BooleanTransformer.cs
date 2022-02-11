using System.Reflection;

namespace AutoAdminLib.Infrastructure.PropertyTransformer.MySql
{
    public class BooleanTransformer : IPropertyTransformer
    {
        public string ForwardTransformValue(PropertyInfo propertyInfo, object propertyValue)
        {
            var propertyValueAsBool = (bool)propertyValue;
            return propertyValueAsBool ? "1" : "0";
        }
    }
}