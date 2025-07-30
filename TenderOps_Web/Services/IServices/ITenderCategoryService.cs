using TenderOps_Web.Models.Dto;

namespace TenderOps_Web.Services.IServices
{
    public interface ITenderCategoryService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(TenderCategoryCreateDTO dto);
        Task<T> UpdateAsync<T>(TenderCategoryUpdateDTO dto);
        Task<T> DeleteAsync<T>(int id);
    }
}
