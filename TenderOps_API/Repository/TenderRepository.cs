using System.Linq.Expressions;
using TenderOps_API.Data;
using TenderOps_API.Models;
using TenderOps_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace TenderOps_API.Repository
{
    public class TenderRepository : Repository<Tender>, ITenderRepository
    {
        private readonly ApplicationDbContext _db;
        public TenderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Tender>> GetTenderPartnerAsync(int partnerId)
        {
            return await _db.Tenders
                .Include(t => t.CreatedByUser)
                .Include(t => t.TenderCategory)
                .Include(t => t.Partner)
                .Where(t => t.PartnerId == partnerId)
                .ToListAsync();
        }

        public async Task<Tender> UpdateAsync(Tender entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Tenders.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }




    }
}
