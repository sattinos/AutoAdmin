using System.Reflection;

namespace AutoAdmin.Infrastructure.PropertyTransformer.MySql
{
    public class GuidTransformer : IPropertyTransformer
    {
        public string ForwardTransformValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return $"UUID_TO_BIN('{propertyValue}')";
        }
    }
}