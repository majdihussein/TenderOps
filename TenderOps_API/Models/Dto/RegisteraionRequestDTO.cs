namespace TenderOps_API.Models.Dto
{
    public class RegisteraionRequestDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string RegistrationNumber { get; set; }
        public int PartnerId { get; set; }
    }
}
