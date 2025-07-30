using System.ComponentModel.DataAnnotations;
using static TenderOps_Utility.SD;

namespace TenderOps_Web.Models.Dto
{
    public class TenderUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string TenderName { get; set; }
        [Required]
        public string TenderNumber { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public TenderLocation Location { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime TenderStartDate { get; set; }
        public DateTime TenderEndDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "Tender category is required")]
        public int TenderCategoryId { get; set; }
        [Required]
        public int PartnerId { get; set; }
    }
}
