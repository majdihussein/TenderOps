using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static TenderOps_Utility.SD;

namespace TenderOps_API.Models
{
    public class Tender
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Invoice? Invoice { get; set; }
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
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? TenderStartDate { get; set; }
        public DateTime? TenderEndDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
        [Required]
        public int TenderCategoryId { get; set; }
        public TenderCategory? TenderCategory { get; set; }

        public int PartnerId { get; set; }
        public Partner Partner { get; set; }

    }
}
