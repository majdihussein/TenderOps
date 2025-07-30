using System.Linq.Expressions;
using TenderOps_API.Models;

namespace TenderOps_API.Repository.IRepository
{
    public interface IPartnerRepository : IRepository<Partner>
    {
        Task<Partner> UpdateAsync(Partner entity);

    }
}
