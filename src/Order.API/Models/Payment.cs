using System;
namespace Order.API.Models
{
    public class Payment
    {
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
    }
}
