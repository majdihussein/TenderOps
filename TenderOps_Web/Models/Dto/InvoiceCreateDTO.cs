using System.ComponentModel.DataAnnotations;

namespace TenderOps_Web.Models.Dto
{
    public class InvoiceCreateDTO
    {
        [Required]
        public string InvoiceNumber { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string? Notes { get; set; } // اختيارية
        public int TenderId { get; set; }


        public string CreatedBy { get; set; }


        public string UserId { get; set; }

    }
}
