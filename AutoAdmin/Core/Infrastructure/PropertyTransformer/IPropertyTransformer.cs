using System.Reflection;

namespace AutoAdmin.Core.Infrastructure.PropertyTransformer
{
    public interface IPropertyTransformer
    {
        string ForwardTransformValue(PropertyInfo propertyInfo, object propertyValue);
    }
}