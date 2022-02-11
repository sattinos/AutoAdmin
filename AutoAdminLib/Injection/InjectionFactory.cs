using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoAdminLib.Injection.Attributes;
using AutoAdminLib.Reflection;

namespace AutoAdminLib.Injection
{
    public interface IConfigurableMiddleWare
    {
        void HttpPipelineConfigure(IApplicationBuilder app, IWebHostEnvironment env);
    }

    public static class InjectionFactory
    {
        public static void StartInjection(IServiceCollection services, IConfiguration configuration, Assembly assembly = null)
        {
            InjectConfigurationSections(services, configuration, assembly);
            InjectServices(services, assembly);
        }

        private static void InjectConfigurationSections(IServiceCollection services, IConfiguration configuration, Assembly assembly = null)
        {
            var types = ReflectionExtension.GetTypesWithAttribute<ConfigurationSectionAttribute>(assembly);
            foreach (var type in types)
            {
                var sectionAttribute = type.GetCustomAttribute<ConfigurationSectionAttribute>();
                if (sectionAttribute != null)
                {
                    var configurationSection = configuration.GetSection(sectionAttribute.KeyName);
                    var configureMethodType = typeof(OptionsConfigurationServiceCollectionExtensions);
                    var configureMethod = configureMethodType.GetMethods()
                        .First(x => x.Name == "Configure" && x.GetParameters().Length == 2);
                    MethodInfo method = configureMethod.MakeGenericMethod(type);
                    method.Invoke(services, new object[] {services, configurationSection});
                    
                    Console.WriteLine($" ===> Configuration section: {configurationSection.Value}");
                }
            }
        }

        private static void InjectServices(IServiceCollection services, Assembly assembly = null)
        {
            var types = ReflectionExtension.GetTypesWithAttribute<InjectAsAttribute>(assembly);
            var serviceTypeMap = new Dictionary<ServiceLifetime, Action<Type>>
            {
                [ServiceLifetime.Singleton] = (type) => services.AddSingleton(type),
                [ServiceLifetime.Transient] = (type) => services.AddTransient(type),
                [ServiceLifetime.Scoped] = (type) => services.AddScoped(type)
            };
            
            foreach (var type in types)
            {
                var injectAsAttribute = type.GetCustomAttribute<InjectAsAttribute>();
                if (injectAsAttribute != null)
                {
                    Console.WriteLine($"Type: {type.Name} has been injected as: {injectAsAttribute.ServiceLifetime}");
                    serviceTypeMap[injectAsAttribute.ServiceLifetime](type);
                }
            }
        }

        private static void RegisterInterfaceImplementations<TInterfaceType>(IServiceCollection services, ServiceLifetime serviceLifetime,Assembly assembly = null)
        {
            var typeInfos = ReflectionExtension.GetInterfacesTypeInfo<TInterfaceType>(assembly);
            foreach (var typeInfo in typeInfos)
            {
                services.Add(new ServiceDescriptor(typeInfo, typeInfo, serviceLifetime));
            }
        }
    }
}