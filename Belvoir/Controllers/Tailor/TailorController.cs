using Belvoir.DTO.Tailor;
using Belvoir.Models;
using Belvoir.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Belvoir.Controllers.Tailor
{
    [Route("api/[controller]")]
    [ApiController]
    public class TailorController : ControllerBase
    {
        private readonly ITailorservice _tailorService;

        public TailorController(ITailorservice tailorService)
        {
            _tailorService = tailorService;
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var user =User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier);

            if (user== null)
            {
                return Unauthorized("Please login"); 
            }

            var response = await _tailorService.GET_ALL_TASK(Guid.Parse(user.Value));
            if (response.statuscode == 200)
                return Ok(response);

            return StatusCode(response.statuscode, response);
        }


        [HttpPut("tasks/{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] string status)
        {
            

            var response = await _tailorService.UpdateStatus(taskId, status);
            return StatusCode(response.statuscode, response);
        }

        [HttpGet("tailordashboard")]
        public async Task<IActionResult> GetDashboard()
        {

            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (user == null)
            {
                return Unauthorized("Please login");
            }
            var response = await _tailorService.GetDashboardapi(Guid.Parse(user.Value));
            return StatusCode(response.statuscode, response);
        }

        [HttpGet("tailorprofile")]
        public async Task<IActionResult> GetTailorProfile()
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (user == null)
            {
                return Unauthorized("Please login");
            }
            var response = await _tailorService.GetTailorprofile(Guid.Parse(user.Value));
            return StatusCode(response.statuscode, response);
        }


        [HttpPost("/tailor/resetpassword")]
        public async Task<IActionResult> ResetTailorPassword([FromBody] PasswordResetDTO data)
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (user == null)
            {
                return Unauthorized("Please login");
            }
            var response = await _tailorService.ResetPassword(Guid.Parse(user.Value),data);
            return StatusCode(response.statuscode, response);
        }
    }
}
