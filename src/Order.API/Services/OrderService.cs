using System;
using System.Threading.Tasks;
using EventBus.Sqs;
using Microsoft.Extensions.Logging;
using Order.API.Events;
using Order.API.ExternalServices;

namespace Order.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IPaymentExternalService paymentExternalService;
        private readonly IEventBus eventBus;
        private readonly ILogger<OrderService> logger;

        public OrderService(IPaymentExternalService paymentExternalService,
                            IEventBus eventBus,
                            ILogger<OrderService> logger)
        {
            this.paymentExternalService = paymentExternalService;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        public async Task Create(Models.Order order)
        {
            logger.LogInformation($"::start call payment service by order id {order.Id} {DateTime.Now}");

            await paymentExternalService.Create(new Models.Payment
            {
                Amount = order.Amount,
                OrderId = order.Id
            });

            logger.LogInformation($"::sent message to delivery service by order id {order.Id} {DateTime.Now}");

            await eventBus.Enqueue(new DeliveryInEvent(order.Id));
        }
    }
}
