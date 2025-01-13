using Belvoir.Bll.DTO.Rental;
using Belvoir.Bll.Services.Rentals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var user =User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier);
            var response = await _service.AddRental(files,rentalData,Guid.Parse(user.Value));
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

        [HttpDelete("")]
        public async Task<IActionResult> DeleteRental(Guid id)
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var response = await _service.DeleteRental(id,Guid.Parse(user.Value));
            return StatusCode(response.statuscode, response);

        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateRental(Guid rentalId, IFormFile[] files, [FromForm] RentalSetDTO rentalData)
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var response = await _service.UpdateRental(rentalId,files,rentalData,Guid.Parse(user.Value));
            return StatusCode(response.statuscode, response);

        }

        [HttpGet("category")]
        public async Task<IActionResult> RentalByCategory(string gender,
        string garmentType,
        string fabricType)
        {
            var response = await _service.GetRentalsByCategory(gender,garmentType,fabricType);
            return StatusCode(response.statuscode, response);

        }

    }
}
