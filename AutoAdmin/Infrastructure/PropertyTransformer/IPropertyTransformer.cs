using System.Reflection;

namespace AutoAdmin.Infrastructure.PropertyTransformer
{
    public interface IPropertyTransformer
    {
        string ForwardTransformValue(PropertyInfo propertyInfo, object propertyValue);
    }
}