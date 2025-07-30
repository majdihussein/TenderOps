using System.Linq.Expressions;
using TenderOps_API.Models;

namespace TenderOps_API.Repository.IRepository
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<Invoice> UpdateAsync(Invoice entity);

    }
}
