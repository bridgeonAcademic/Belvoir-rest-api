using Belvoir.Bll.DTO.Order;
using Belvoir.Bll.Services.Admin;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


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
            //string user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).ToString();
            //Guid userId = Guid.TryParse(user, out Guid parsedGuid) ? parsedGuid : Guid.Empty;
            var response = await _orderServices.AddTailorProducts(tailorProductDTO);
            return StatusCode(statusCode: response.StatusCode, response);
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> AddOrder(Order order)
        {
            //string user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).ToString();
            //Guid userId = Guid.TryParse(user, out Guid parsedGuid) ? parsedGuid : Guid.Empty;
            //order.customerId = userId;
            var response = await _orderServices.AddOrder(order);
            return StatusCode(statusCode: response.StatusCode, response);
        }
    }
}
