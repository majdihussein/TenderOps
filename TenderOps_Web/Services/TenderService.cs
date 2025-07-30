using System;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;

namespace TenderOps_Web.Services
{
    public class TenderService : ITenderService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string tenderUrl;

        public TenderService(IHttpClientFactory clientFactory, IConfiguration configuration
            , IBaseService baseService)
        {
            _clientFactory = clientFactory;
            tenderUrl = configuration.GetValue<string>("ServiceUrls:API");
            _baseService = baseService;
        }

        public async Task<T> CreateAsync<T>(TenderCreateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderAPI",
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderAPI/" + id
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderAPI?pagesize=0"
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderAPI/" + id
            });
        }

        public async Task<T> UpdateAsync<T>(TenderUpdateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderAPI/" + dto.Id,
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<T> GetTenderPartner<T>(int partnerId)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = tenderUrl + $"/api/{SD.CurrentAPIVersion}/TenderAPI/GetTenderPartner/{partnerId}"
            });

        }
    }
}
