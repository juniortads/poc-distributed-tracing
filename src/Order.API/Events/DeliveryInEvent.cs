using System;
using EventBus.Sqs.Events;

namespace Order.API.Events
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
