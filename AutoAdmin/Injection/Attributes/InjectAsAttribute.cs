using System;
using Microsoft.Extensions.DependencyInjection;

namespace AutoAdmin.Injection.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectAsAttribute : Attribute
    {
        public ServiceLifetime ServiceLifetime { get; }

        public InjectAsAttribute(ServiceLifetime serviceLifetime)
        {
            ServiceLifetime = serviceLifetime;
        }
    }
}