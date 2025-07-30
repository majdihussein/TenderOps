using System.ComponentModel.DataAnnotations;

namespace TenderOps_Web.Models.Dto
{
    public class RegisterationRequestDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        public string RegistrationNumber { get; set; }

        public int PartnerId { get; set; }

    }
}
