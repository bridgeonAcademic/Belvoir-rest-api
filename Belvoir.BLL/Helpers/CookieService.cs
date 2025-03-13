using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Helpers
{

    public interface ICookieService
    {
        Task SetCookie(string key, string value, int expirehours);
        Task<string> GetCookie(string key);
        Task DeleteCookie(string key);
    }

    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SetCookie(string key, string value, int expirehours)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = false, // Important for localhost (use true in production)
                SameSite = SameSiteMode.None, // "None" requires Secure=true, change to "Lax" for local testing
                Expires = DateTime.UtcNow.AddHours(expirehours),
                Path = "/" // Ensure cookie is available site-wide
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, cookieOptions);
        }

        public async Task<string> GetCookie(string key)
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[key];
        }

        public async Task DeleteCookie(string key)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
        }
    }

}
