using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;
using Newtonsoft.Json.Linq;

namespace TenderOps_Web.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string invoiceUrl;

        public InvoiceService(IHttpClientFactory clientFactory, IConfiguration configuration
            , IBaseService baseService)
        {
            _clientFactory = clientFactory;
            invoiceUrl = configuration.GetValue<string>("ServiceUrls:API");
            _baseService = baseService;
        }

        public async Task<T> CreateAsync<T>(InvoiceCreateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = invoiceUrl + $"/api/{SD.CurrentAPIVersion}/InvoiceAPI/"
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = invoiceUrl + $"/api/{SD.CurrentAPIVersion}/InvoiceAPI/" + id
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = invoiceUrl + $"/api/{SD.CurrentAPIVersion}/InvoiceAPI"
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = invoiceUrl + $"/api/{SD.CurrentAPIVersion}/InvoiceAPI/" + id
            });
        }
    }
}
