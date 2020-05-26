using System;
namespace Payment.API.Models
{
    public class Payment
    {
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
    }
}
