using System.ComponentModel.DataAnnotations;

namespace TenderOps_API.Models
{
    public class LocalUser
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string RegistrationNumber { get; set; }
        public bool CanPostTenders { get; set; } = false;
    }
}
