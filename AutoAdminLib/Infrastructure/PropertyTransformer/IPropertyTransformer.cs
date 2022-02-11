using System.Reflection;

namespace AutoAdminLib.Infrastructure.PropertyTransformer
{
    public interface IPropertyTransformer
    {
        string ForwardTransformValue(PropertyInfo propertyInfo, object propertyValue);
    }
}