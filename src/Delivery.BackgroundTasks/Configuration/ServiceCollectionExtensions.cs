using System;
using Delivery.BackgroundTasks.Background;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.BackgroundTasks.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopeHostedService<TImplementation>(this IServiceCollection services)
            where TImplementation : IScopedProcessingService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddHostedService<ConsumeScopedServiceHostedService<TImplementation>>();
            services.AddScoped(typeof(TImplementation));

            return services;
        }
    }
}
