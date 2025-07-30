using TenderOps_Web.Models.Dto;

namespace TenderOps_Web.Services.IServices
{
    public interface IInvoiceService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(InvoiceCreateDTO dto);
        Task<T> DeleteAsync<T>(int id);
    }
}
