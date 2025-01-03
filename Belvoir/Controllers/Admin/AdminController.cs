
using Belvoir.Models;
using Belvoir.Services;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly AdminServices _myService;

        public AdminController(AdminServices myService)
        {
            _myService = myService;
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var data = _myService.GetData();
            return Ok(data);
        }





    }
}
