using Belvoir.Bll.Services;
using Belvoir.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;
        public NotificationController(INotificationService service) {
        _service = service;
        }
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotification() {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _service.Get_Notifiation(userId);
            return StatusCode(response.statuscode,response);
        }
    }
}
