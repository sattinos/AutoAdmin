using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoAdminLib.Injection;

namespace AutoAdminLib
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            InjectionFactory.StartInjection(serviceCollection, configuration);
        }
    }
}