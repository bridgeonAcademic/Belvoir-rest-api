using System.ComponentModel.DataAnnotations;

namespace Belvoir.Bll.DTO.Tailor
{
    public class PasswordResetDTO
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

    }
}
