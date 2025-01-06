using Belvoir.Models.Generic_response;
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
            var user =User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier).Value;
            var response = await _tailorService.GET_ALL_TASK(Guid.Parse(user));
            if (response.statuscode == 200)
                return Ok(response);

            return StatusCode(response.statuscode, response);
        }


        [HttpPut("tasks/{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest(new Response<object> { statuscode = 400, message = "Status cannot be empty." });

            var response = await _tailorService.UpdateStatus(taskId, status);
            return StatusCode(response.statuscode, response);
        }

        [HttpGet("tailordashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var response = await _tailorService.GetDashboardapi(Guid.Parse(user));
            return StatusCode(response.statuscode, response);
        }

        [HttpGet("tailorprofile")]
        public async Task<IActionResult> GetTailorProfile()
        {
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var response = await _tailorService.GetTailorprofile(Guid.Parse(user));
            return StatusCode(response.statuscode, response);
        }
    }
}
