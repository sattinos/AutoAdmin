using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoAdminLib.Reflection
{
    public static class ReflectionExtension
    {
        public static IEnumerable<TypeInfo> GetInterfacesTypeInfo<TInterfaceType>(Assembly assembly = null) {
            var interfaceType = typeof(TInterfaceType);
            return (assembly ?? Assembly.GetExecutingAssembly()).DefinedTypes.Where(x => x.GetInterfaces().Contains(interfaceType));
        }

        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(Assembly assembly = null) where TAttribute : Attribute
        {
            var targetAssembly = assembly ?? Assembly.GetExecutingAssembly();
            foreach (Type type in targetAssembly.GetTypes())
            {
                if (type.GetCustomAttribute<TAttribute>(true) != null)
                {
                    yield return type;
                }
            }
        }
    }
}
