using System.ComponentModel.DataAnnotations;

namespace TenderOps_Web.Models.Dto
{
    public class TenderCategoryCreateDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
