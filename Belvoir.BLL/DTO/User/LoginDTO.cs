using System.ComponentModel.DataAnnotations;

namespace Belvoir.Bll.DTO.User
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
