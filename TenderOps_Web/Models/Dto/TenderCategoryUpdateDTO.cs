using System.ComponentModel.DataAnnotations;

namespace TenderOps_Web.Models.Dto
{
    public class TenderCategoryUpdateDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
