using Microsoft.AspNetCore.Identity;

namespace TenderOps_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string RegistrationNumber { get; set; }
        public bool CanPostTenders { get; set; } = false;

        public Partner Partner { get; set; }

        public int? PartnerId { get; set; }
    }
}
