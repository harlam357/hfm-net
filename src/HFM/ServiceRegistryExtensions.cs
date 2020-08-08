using System;

using LightInject;

namespace HFM
{
    public static class ServiceRegistryExtensions
    {
        /// <summary>Allows post-processing of a service instance.</summary>
        /// <param name="processor">An action delegate that exposes the created service instance.</param>
        /// <returns>The <see cref="IServiceRegistry" />, for chaining calls.</returns>
        public static IServiceRegistry Initialize<TService>(this IServiceRegistry serviceRegistry, Action<IServiceFactory, TService> processor)
        {
            return serviceRegistry.Initialize(registration => registration.ServiceType == typeof(TService),
                (factory, instance) => processor(factory, (TService)instance));
        }
    }
}
