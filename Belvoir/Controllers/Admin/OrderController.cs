using Belvoir.Bll.DTO.Order;
using Belvoir.Bll.Services.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _orderServices;
        public OrderController(IOrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        [HttpPost("add/tailorProduct")]
        public  async Task<IActionResult> CreateTailorProduct(TailorProductDTO tailorProductDTO)
        {
            var response = await _orderServices.AddTailorProducts(tailorProductDTO);
            return StatusCode(statusCode: response.StatusCode, response);
        }
    }
}
