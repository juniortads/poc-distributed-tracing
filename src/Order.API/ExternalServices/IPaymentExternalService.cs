using System;
using System.Threading.Tasks;
using Order.API.Models;

namespace Order.API.ExternalServices
{
    public interface IPaymentExternalService
    {
        Task Create(Payment payment);
    }
}
