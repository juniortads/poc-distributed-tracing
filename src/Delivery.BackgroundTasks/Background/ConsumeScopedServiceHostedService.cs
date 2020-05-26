using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Delivery.BackgroundTasks.Background
{
    public class ConsumeScopedServiceHostedService<TImplementation> : BackgroundService
        where TImplementation : IScopedProcessingService
    {
        private IServiceProvider Services { get; }

        private readonly ILogger<ConsumeScopedServiceHostedService<TImplementation>> logger;

        public ConsumeScopedServiceHostedService(IServiceProvider services,
            ILogger<ConsumeScopedServiceHostedService<TImplementation>> logger)
        {
            Services = services;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(100);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await DoWork(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"There was an error while the task. The message was:{ex.GetBaseException().Message}", ex.GetBaseException());
                Environment.Exit(-1);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService(typeof(TImplementation)) as IScopedProcessingService;

                await scopedProcessingService.ExecuteAsync(stoppingToken);
            }
        }
    }
}
