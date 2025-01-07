
using Belvoir.Models;
using Belvoir.Services.Admin;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminServices _myService;

        public AdminController(IAdminServices myService)
        {
            _myService = myService;
        }

        [HttpGet("User")]
        public async Task<IActionResult> GetUsers()
        {
            var data = await _myService.GetAllUsers();
            return Ok(data);
        }

        [HttpGet("user/id")]
        public async Task<IActionResult> GetUsersById(Guid id)
        {
            var data = await _myService.GetUserById(id);
            return Ok(data);
        }

        [HttpGet("user/{name}")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var data = await _myService.GetUserByName(name);
            return Ok(data);
        }

        // PATCH Endpoint for Blocking/Unblocking a User
        [HttpPatch("user/block-unblock/{id}")]
        public async Task<IActionResult> BlockOrUnblockUser(Guid id)
        {
            var response = await _myService.BlockOrUnblock(id);
            if (response.statuscode == 400)
            {
                return BadRequest(response);
            }
            return StatusCode(response.statuscode, response);
        }

    }
}
