using Belvoir.Bll.DTO.User;
using Belvoir.Bll.Helpers;
using Belvoir.Bll.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Belvoir.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly ICookieService _cookieServices;

        public AuthController(IAuthServices authServices, ICookieService cookieServices)
        {
            _authServices = authServices;
            _cookieServices = cookieServices;
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authServices.RegisterUserAsync(registerDTO);
            return StatusCode(response.StatusCode, response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var response = await _authServices.LoginAsync(loginDTO);
            if (response.StatusCode == 200)
            {
                await _cookieServices.SetCookie("jwt", response.Data.AccessToken, 3);
                await _cookieServices.SetCookie("role", response.Data.Role, 3);
            }
            
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _cookieServices.DeleteCookie("jwt");
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> GetRefreshToken(string refreshtoken)
        {
            var response = await _authServices.RefreshTokenAsync(refreshtoken);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-role")]
        [Authorize]
        public IActionResult GetRole()
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roleClaim == null)
            {
                return Unauthorized(new { message = "Role not found in token" });
            }

            return Ok(new { role = roleClaim.Value });
        }

        [HttpGet("get-token")]
        public IActionResult GetTokenFromCookie()
        {
            var token = _cookieServices.GetCookie("jwt").Result;
            var role = _cookieServices.GetCookie("role").Result;

            if (token != null && role != null)
            {
                return Ok(new { Token = token,Role =  role});
            }

            return NotFound("JWT Token not found in cookies.");
        }
    }
}