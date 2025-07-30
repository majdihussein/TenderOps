using TenderOps_Web.Models.Dto;

namespace TenderOps_Web.Services.IServices
{
    public interface ITenderService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(TenderCreateDTO dto);
        Task<T> UpdateAsync<T>(TenderUpdateDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetTenderPartner<T>(int partnerId);
    }
}
