using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;

namespace TenderOps_Web.Services
{
    public class TenderCategoryService : ITenderCategoryService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string tenderUrl;

        public TenderCategoryService(IHttpClientFactory clientFactory, IConfiguration configuration
            , IBaseService baseService)
        {
            _clientFactory = clientFactory;
            tenderUrl = configuration.GetValue<string>("ServiceUrls:API");
            _baseService = baseService;
        }

        public async Task<T> CreateAsync<T>(TenderCategoryCreateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderCategoryAPI",
                //ContentType = SD.ContentType.MultipartFormData
            });
        }
        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderCategoryAPI/" + id
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderCategoryAPI?pagesize=0"
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderCategoryAPI/" + id
            });
        }

        public async Task<T> UpdateAsync<T>(TenderCategoryUpdateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderCategoryAPI/" + dto.Id,
            });
        }
    }
}
