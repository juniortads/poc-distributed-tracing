using System;
using System.Threading;
using System.Threading.Tasks;
using Delivery.BackgroundTasks.Background;
using Delivery.BackgroundTasks.Events;
using EventBus.Sqs;
using Microsoft.Extensions.Logging;

namespace Delivery.BackgroundTasks.Tasks
{
    public class DeliveryTask : IScopedProcessingService
    {
        private ILogger<DeliveryTask> logger;
        private readonly IEventBus eventBus;

        public DeliveryTask(ILogger<DeliveryTask> logger, IEventBus eventBus)
        {
            this.logger = logger;
            this.eventBus = eventBus;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"::delivery task executeAsync {DateTime.Now}");

            var deliveryInEvent = eventBus.ReceiveMessage<DeliveryInEvent>(5);

            if(deliveryInEvent != null)
            {
                logger.LogInformation($"::receive message event {deliveryInEvent.Id} by order {deliveryInEvent.OrderId}");

                await eventBus.Dequeue(deliveryInEvent);
            }

            await Task.Delay(1000);
        }
    }
}
