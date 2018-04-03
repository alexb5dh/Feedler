using System;
using System.Linq;
using Feedler.Extensions.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Feedler.Tests.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Replaces <paramref name="oldDescriptor"/> with <paramref name="newDescriptor"/>.
        /// </summary>
        public static bool Replace(this IServiceCollection services, ServiceDescriptor oldDescriptor, ServiceDescriptor newDescriptor)
        {
            var oldIndex = services.IndexOf(oldDescriptor);
            if (oldIndex < 0) return false;

            services[oldIndex] = newDescriptor;
            return true;
        }

        /// <summary>
        /// Removes first descriptor for <see cref="TService"/> if present.
        /// </summary>
        public static bool Remove<TService>(this IServiceCollection services)
        {
            var index = services.IndexOf(d => d.ServiceType == typeof(TService));

            if (index >= 0)
            {
                services.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Replaces first <paramref name="serviceType"/> singleton implementation.
        /// </summary>
        public static bool ReplaceSingleton(this IServiceCollection services, Type serviceType, object implementation)
        {
            var oldService = services.FirstOrDefault(sd => sd.ServiceType == serviceType && sd.Lifetime == ServiceLifetime.Singleton);
            if (oldService == null) return false;

            var newService = new ServiceDescriptor(oldService.ServiceType, implementation);
            return services.Replace(oldService, newService);
        }

        /// <summary>
        /// Replaces first <typeparamref name="TService"/> singleton implementation.
        /// </summary>
        public static bool ReplaceSingleton<TService>(this IServiceCollection services, TService implementation) =>
            services.ReplaceSingleton(typeof(TService), implementation);
    }
}