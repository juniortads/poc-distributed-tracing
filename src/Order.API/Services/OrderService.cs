using System;
using System.Threading.Tasks;
using Order.API.ExternalServices;

namespace Order.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IPaymentExternalService paymentExternalService;

        public OrderService(IPaymentExternalService paymentExternalService)
        {
            this.paymentExternalService = paymentExternalService;
        }

        public async Task Create(Models.Order order)
        {
            await paymentExternalService.Create();
        }
    }
}
