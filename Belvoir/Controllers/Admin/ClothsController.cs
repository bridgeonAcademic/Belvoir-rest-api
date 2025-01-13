using Belvoir.Bll.Services.Admin;
using Belvoir.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClothsController : ControllerBase
    {
        private readonly IClothsServices _myService;

        public ClothsController(IClothsServices myService)
        {
            _myService = myService;
        }

        [HttpGet("cloths")]
        public async Task<IActionResult> GetUsers()
        {
            var data = await _myService.GetAllCloths();
            return Ok(data);
        }

        [HttpGet("cloth/id")]
        public async Task<IActionResult> GetUsersById(Guid id)
        {
            var data = await _myService.GetClothById(id);
            return Ok(data);
        }

        [HttpGet("cloths/{name}")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var data = await _myService.GetClothsByName(name);
            return Ok(data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCloths(Cloth cloth)
        {
            var data = await _myService.UpdateCloths(cloth);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> AddCloths(IFormFile file,[FromForm] Cloth cloth)
        {
            var data = await _myService.AddCloths(file,cloth);
            return Ok(data);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCloths(Guid id)
        {
            var data = await _myService.DeleteCloths(id);
            return Ok(data);
        }
    }
}
