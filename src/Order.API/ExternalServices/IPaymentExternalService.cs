using System;
using System.Threading.Tasks;

namespace Order.API.ExternalServices
{
    public interface IPaymentExternalService
    {
        Task Create();
    }
}
