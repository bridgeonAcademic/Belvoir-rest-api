using System.ComponentModel.DataAnnotations;

namespace Belvoir.DTO.Tailor
{
    public class TailorDTO
    {
        [Required] public string Name { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Phone { get; set; }
        [Required] public int Experience { get; set; }
    }
}
