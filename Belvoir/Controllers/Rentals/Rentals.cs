using Belvoir.DTO.Rental;
using Belvoir.Services.Rentals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Rentals
{
    [Route("api/[controller]")]
    [ApiController]
    public class Rentals : ControllerBase
    {
        private readonly IRentalService _service;
        public Rentals(IRentalService service) { 
        _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddRental([FromForm] RentalSetDTO rentalData)
        {

            var response = await _service.AddRental(rentalData);
            return StatusCode(response.statuscode,response);

        }
    }
}
