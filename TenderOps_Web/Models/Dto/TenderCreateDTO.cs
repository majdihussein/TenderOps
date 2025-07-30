using System.ComponentModel.DataAnnotations;
using static TenderOps_Utility.SD;

namespace TenderOps_Web.Models.Dto
{
    public class TenderCreateDTO
    {
        [Required]
        public string TenderName { get; set; }
        [Required]
        public string TenderNumber { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public TenderLocation Location { get; set; }
        [Required]
        public string Status { get; set; }  // ممكن تحدد قيم معينة لاحقاً
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public DateTime? TenderStartDate { get; set; }
        public DateTime? TenderEndDate { get; set; }
        public string? CreatedByUserId { get; set; }  // مهم عشان تعرف مين المنشئ
        [Required]
        public int TenderCategoryId { get; set; }
        [Required]
        public int PartnerId { get; set; }

    }
}
