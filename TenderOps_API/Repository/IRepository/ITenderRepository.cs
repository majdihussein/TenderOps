using System.Linq.Expressions;
using TenderOps_API.Models;

namespace TenderOps_API.Repository.IRepository
{
    public interface ITenderRepository : IRepository<Tender>
    {
        Task<Tender> UpdateAsync(Tender entity);
        Task<IEnumerable<Tender>> GetTenderPartnerAsync(int partnerId);

    }
}
