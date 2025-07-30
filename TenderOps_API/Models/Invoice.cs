using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TenderOps_Utility.SD;

namespace TenderOps_API.Models
{
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceId { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }
        [Required]
        public int TenderId { get; set; }
        [ForeignKey("TenderId")]
        public Tender Tender { get; set; }
        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public DateTime InvoiceDate { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public InvoiceStatus Status { get; set; }
        [Required]
        public string Notes { get; set; }
    }
}
