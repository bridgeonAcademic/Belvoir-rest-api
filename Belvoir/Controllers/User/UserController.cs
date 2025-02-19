using Belvoir.Bll.Services.UserSer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Belvoir.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IuserService _userService;
        public UserController(IuserService userService)
        {
            _userService = userService;   
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "User")]
        [HttpGet("profile-User")]
        public async Task<IActionResult> GetUserProfile()
        {
            Guid userId = Guid.Parse(HttpContext.Items["UserId"].ToString());
            var response = await _userService.GetUserProfile(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
