using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AutoAdmin.Core.Reflection;
using AutoAdmin.Injection.Attributes;

namespace AutoAdmin.Injection
{
    public interface ITransientService
    {
    }

    public interface ISingletonService
    {
    }

    public interface IScopedService
    {
    }

    public interface IConfigurableMiddleWare
    {
        void HttpPipelineConfigure(IApplicationBuilder app, IWebHostEnvironment env);
    }

    public static class InjectionFactory
    {
        public static void StartInjection(IServiceCollection services, IConfiguration configuration, Assembly assembly = null)
        {
            // InjectAssemblyServices(services, assembly);
            InjectServices(services, assembly);
            InjectConfigurationSections(services, configuration, assembly);
        }

        private static void InjectAssemblyServices(IServiceCollection services, Assembly assembly = null)
        {
            RegisterInterfaceImplementations<ITransientService>(services, ServiceLifetime.Transient, assembly);
            RegisterInterfaceImplementations<IScopedService>(services, ServiceLifetime.Scoped, assembly);
            RegisterInterfaceImplementations<ISingletonService>(services, ServiceLifetime.Singleton, assembly);
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