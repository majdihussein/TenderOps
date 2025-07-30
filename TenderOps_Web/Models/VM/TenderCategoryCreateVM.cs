using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using TenderOps_Web.Models.Dto;

namespace TenderOps_Web.Models.VM
{
    public class TenderCategoryCreateVM
    {
        [Required(ErrorMessage = "Category Required")]
        [Display(Name = "TenderCategory")]
        public string Name { get; set; }
    }
}
