using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;

namespace TenderOps_Web.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string partnerUrl;

        public PartnerService(IHttpClientFactory clientFactory, IConfiguration configuration
            , IBaseService baseService)
        {
            _clientFactory = clientFactory;
            partnerUrl = configuration.GetValue<string>("ServiceUrls:API");
            _baseService = baseService;
        }

        public async Task<T> CreateAsync<T>(PartnerCreateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = partnerUrl + $"/api/{SD.CurrentAPIVersion}/PartnerAPI",
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = partnerUrl + $"/api/{SD.CurrentAPIVersion}/PartnerAPI/" + id
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = partnerUrl + $"/api/{SD.CurrentAPIVersion}/PartnerAPI?pagesize=0"
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = partnerUrl + $"/api/{SD.CurrentAPIVersion}/PartnerAPI/" + id
            });
        }

        public async Task<T> UpdateAsync<T>(PartnerUpdateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = partnerUrl + $"/api/{SD.CurrentAPIVersion}/PartnerAPI/" + dto.Id,
            });
        }
    }
}
