using System;
using System.Threading.Tasks;

namespace Order.API.Services
{
    public interface IOrderService
    {
        Task Create(Models.Order order);
    }
}
