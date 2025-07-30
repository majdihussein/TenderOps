using TenderOps_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TenderOps_Web.Models.VM
{
    public class InvoiceCreateVM
    {
        public InvoiceCreateDTO InvoiceCreateDTO { get; set; }

        // قائمة العطاءات لاختيار العطاء المرتبط بالفاتورة
        public IEnumerable<SelectListItem> TenderList { get; set; }
    }
}
