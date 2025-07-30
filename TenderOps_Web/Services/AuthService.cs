using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;

namespace TenderOps_Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string tenderUrl;

        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseService baseService)

        {
            _clientFactory = clientFactory;
            tenderUrl = configuration.GetValue<string>("ServiceUrls:API");
            _baseService = baseService;
        }
        public async Task<T> LoginAsync<T>(LoginRequestDTO objToCreate)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = objToCreate,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/UsersAuth/login"
            },withBearer:false);
        }

        public async Task<T> LogoutAsync<T>(TokenDTO obj)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/UsersAuth/revoke"
            });
        }

        public async Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/UsersAuth/register"
            }, withBearer: false);
        }
    }
}
