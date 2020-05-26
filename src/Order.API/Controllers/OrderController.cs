using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Order.API.Services;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Models.Order order)
        {
            await orderService.Create(order);

            return Ok();
        }
    }
}
