namespace TenderOps_API.Models.Dto
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int? PartnerId { get; set; }
        public bool CanPostTenders { get; set; }

    }
}
