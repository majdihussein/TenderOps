using System.ComponentModel.DataAnnotations;

namespace TenderOps_Web.Models.Dto
{
    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string TenderName { get; set; } // من العلاقة مع Tender
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string CreatedBy { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
    }
}
