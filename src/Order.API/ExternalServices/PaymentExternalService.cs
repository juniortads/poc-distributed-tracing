using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Order.API.Exceptions;
using Order.API.Models;

namespace Order.API.ExternalServices
{
    public class PaymentExternalService : IPaymentExternalService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<PaymentExternalService> logger;

        public PaymentExternalService(IHttpClientFactory clientFactory, ILogger<PaymentExternalService> logger)
        {
            this.clientFactory = clientFactory;
            this.logger = logger;
        }

        public async Task Create(Payment payment)
        {
            try
            {
                var client = clientFactory.CreateClient("PaymentServiceClient");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{client.BaseAddress}payment")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(httpRequest);

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                string message = $"An error occurred while connection with payments service. {ex.GetBaseException().Message}";

                logger.LogError(ex.GetBaseException(), message);

                throw new OrderConnectionException(message, ex.GetBaseException());
            }
        }
    }
}
