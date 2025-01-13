using System.ComponentModel.DataAnnotations;

namespace Belvoir.Bll.DTO.User
{
    public class RegisterDTO
    {
        [Required] public string Name { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Phone { get; set; }
    }
}
