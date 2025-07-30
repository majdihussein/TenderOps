using System.Linq.Expressions;
using TenderOps_API.Data;
using TenderOps_API.Models;
using TenderOps_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace TenderOps_API.Repository
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        private readonly ApplicationDbContext _db;
        public InvoiceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Invoice> UpdateAsync(Invoice entity)
        {
            entity.InvoiceDate = DateTime.Now;
            _db.Invoices.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
