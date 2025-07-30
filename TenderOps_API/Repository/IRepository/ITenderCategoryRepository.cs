using System.Linq.Expressions;
using TenderOps_API.Models;

namespace TenderOps_API.Repository.IRepository
{
    public interface ITenderCategoryRepository : IRepository<TenderCategory>
    {
        Task<TenderCategory> UpdateAsync(TenderCategory entity);

    }
}
