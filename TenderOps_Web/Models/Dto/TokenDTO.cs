using Newtonsoft.Json;

namespace TenderOps_Web.Models.Dto
{
    public class TokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
