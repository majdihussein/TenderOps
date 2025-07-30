using System.ComponentModel.DataAnnotations;

namespace TenderOps_Web.Models.Dto
{
    public class PartnerCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
        public string? Website { get; set; }
    }
}
