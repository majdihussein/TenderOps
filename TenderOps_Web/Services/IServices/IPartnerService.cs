using TenderOps_Web.Models.Dto;

namespace TenderOps_Web.Services.IServices
{
    public interface IPartnerService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(PartnerCreateDTO dto);
        Task<T> DeleteAsync<T>(int id);
        Task<T> UpdateAsync<T>(PartnerUpdateDTO dto);
    }
}
