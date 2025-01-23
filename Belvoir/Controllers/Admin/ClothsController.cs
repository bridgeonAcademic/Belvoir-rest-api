using Belvoir.Bll.Services.Admin;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClothesController : ControllerBase
    {
        private readonly IClothsServices _myService;

        public ClothesController(IClothsServices myService)
        {
            _myService = myService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetClothes([FromQuery] ProductQuery pquery)
        {
            var data = await _myService.GetAllCloths(pquery);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClothesById(Guid id)
        {
            var data = await _myService.GetClothById(id);
            return Ok(data);
        }

        

        [HttpPut]
        public async Task<IActionResult> UpdateCloths(Cloth cloth)
        {
            var data = await _myService.UpdateCloths(cloth);
            return Ok(data);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddCloths(IFormFile file,[FromForm] Cloth cloth)
        {
            var data = await _myService.AddCloths(file,cloth);
            return Ok(data);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCloths(Guid id)
        {
            var data = await _myService.DeleteCloths(id);
            return Ok(data);
        }


        //[HttpPost("whishlist")]
        //public async Task<IActionResult> AddToWhisList(Guid productid)
        //{
        //    var data = await _myService.AddWishlist(userid,productid);
        //    return StatusCode(data.stat);
        //}
    }
}
