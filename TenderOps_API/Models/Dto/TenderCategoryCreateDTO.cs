using System.ComponentModel.DataAnnotations;

namespace TenderOps_API.Models.Dto
{
    public class TenderCategoryCreateDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
