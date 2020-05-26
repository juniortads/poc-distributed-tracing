using System;
using EventBus.Sqs.Events;

namespace Delivery.BackgroundTasks.Events
{
    public class DeliveryInEvent : IntegrationEvent
    {
        public string OrderId { get; set; }

        public DeliveryInEvent(string id) : base(id, DateTime.UtcNow)
        {
            OrderId = id;
        }
    }
}
