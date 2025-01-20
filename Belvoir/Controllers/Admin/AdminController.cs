
using Belvoir.Bll.DTO.Tailor;
using Belvoir.Bll.DTO.User;
using Belvoir.DAL.Models;
using Belvoir.Bll.Services.Admin;
using CloudinaryDotNet.Actions;
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

        [HttpGet("users/{role}")]
        public async Task<IActionResult> GetUsers(string role,[FromQuery] UserQuery userQuery)
        {
            var data = await _myService.GetAllUsers(role,userQuery);
            return Ok(data);
        }

        [HttpGet("user/id/{id}")]
        public async Task<IActionResult> GetUsersById(Guid id)
        {
            var data = await _myService.GetUserById(id);
            return Ok(data);
        }

        

        // PATCH Endpoint for Blocking/Unblocking a User
        [HttpPatch("{role}/block-unblock/{id}")]
        public async Task<IActionResult> BlockOrUnblockUser(Guid id,string role)
        {
            var response = await _myService.BlockOrUnblock(id,role);
            if (response.statuscode == 400)
            {
                return BadRequest(response);
            }
            return StatusCode(response.statuscode, response);
        }
        [HttpPost("add/tailor")]
        public async Task<IActionResult> AddTailor([FromBody] TailorDTO tailorDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _myService.AddTailor(tailorDTO);
            return StatusCode(response.statuscode, response);
        }

        [HttpDelete("delete/tailor")]
        public async Task<IActionResult> DeleteTailor(Guid id)
        {
            var response = await _myService.DeleteTailor(id);
            return StatusCode(response.statuscode, response);
        }

        [HttpGet("sales-report")]

        public async Task<IActionResult> SalesReport()
        {
            var respose =await _myService.GetSalesReport();
            return StatusCode(respose.statuscode, respose);
        }


        [HttpGet("Dashboard")]

        public async Task<IActionResult> Dashboarddata()
        {
            var respose = await _myService.GetDasboard();
            return StatusCode(respose.statuscode, respose);
        }
    }
}
