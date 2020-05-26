using System;
namespace Order.API.Models
{
    public class Order
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
