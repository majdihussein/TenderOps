using System.ComponentModel.DataAnnotations;
using static TenderOps_Utility.SD;

namespace TenderOps_Web.Models.Dto
{
    public class TenderDTO
    {
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
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        public DateTime? TenderStartDate { get; set; }
        public DateTime? TenderEndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public InvoiceDTO? Invoice { get; set; }
        public int TenderCategoryId { get; set; }
        public string? TenderCategoryName { get; set; }

        public int PartnerId { get; set; }
        public string? PartnerName { get; set; }

    }
}
