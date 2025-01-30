using Belvoir.Bll.DTO.Rental;
using Belvoir.Bll.Services.Rentals;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Belvoir.Controllers.Rentals
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalCartController : ControllerBase
    {
        private readonly IRentalCartService _service;

        public RentalCartController(IRentalCartService service)
        {
            _service = service;
        }

        [HttpGet("my-cart")]
        public async Task<IActionResult> GetCart()
        {

                Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());

                var response = await _service.GetCartByUserId(userId);

                return StatusCode(response.StatusCode, response);
        }

        [HttpPost("AddToCart")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO cartDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    StatusCode = 400,
                    Message = "Invalid input",
                    Error = "Invalid request payload",
                    Data = null
                });
            }

            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());

            if (userId == Guid.Empty)
            {
                return Unauthorized(new Response<string>
                {
                    StatusCode = 401,
                    Message = "Unauthorized",
                    Error = "User ID is missing",
                    Data = null
                });
            }

            var response = await _service.AddToCartAsync(userId, cartDTO);

            return StatusCode(response.StatusCode, response);
        }

    }

}
