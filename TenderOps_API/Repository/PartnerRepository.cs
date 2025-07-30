using System.Linq.Expressions;
using TenderOps_API.Data;
using TenderOps_API.Models;
using TenderOps_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace TenderOps_API.Repository
{
    public class PartnerRepository : Repository<Partner>, IPartnerRepository
    {
        private readonly ApplicationDbContext _db;
        public PartnerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Partner> UpdateAsync(Partner entity)
        {
            _db.Partners.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
