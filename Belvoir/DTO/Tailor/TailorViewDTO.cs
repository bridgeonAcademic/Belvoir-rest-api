using System.ComponentModel.DataAnnotations;

namespace Belvoir.DTO.Tailor
{
    public class TailorViewDTO
    {
        [Required] public string Name { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Phone { get; set; }

        public string experince { get; set; }
    }
}
