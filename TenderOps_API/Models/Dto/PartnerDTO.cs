using System.ComponentModel.DataAnnotations;

namespace TenderOps_API.Models.Dto
{
    public class PartnerDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
        public string? Website { get; set; }
    }
}
