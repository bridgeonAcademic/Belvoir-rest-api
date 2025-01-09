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
        public async Task<IActionResult> AddRental(IFormFile[] files ,[FromForm] RentalSetDTO rentalData)
        {

            var response = await _service.AddRental(files,rentalData);
            return StatusCode(response.statuscode,response);

        }

        [HttpGet("id")]
        public async Task<IActionResult> SearchRentalid(Guid id)
        {

            var response = await _service.GetRentalById(id);
            return StatusCode(response.statuscode, response);

        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRental(string name)
        {

            var response = await _service.SearchRental(name);
            return StatusCode(response.statuscode, response);

        }

        [HttpGet("paginated")]
        public async Task<IActionResult> Paginated(int pagenumber,int pagesize)
        {

            var response = await _service.PaginatedProduct(pagenumber,pagesize);
            return StatusCode(response.statuscode, response);

        }

    }
}
