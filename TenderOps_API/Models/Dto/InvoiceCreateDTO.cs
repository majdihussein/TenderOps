using System.ComponentModel.DataAnnotations;

namespace TenderOps_API.Models.Dto
{
    public class InvoiceCreateDTO
    {
        [Required]
        public string InvoiceNumber { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string? Notes { get; set; } // اختيارية
        [Required(ErrorMessage = "TenderId is required")]
        public int TenderId { get; set; }

    }
}
