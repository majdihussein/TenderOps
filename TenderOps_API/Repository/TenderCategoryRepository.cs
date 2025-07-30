using System.Linq.Expressions;
using TenderOps_API.Data;
using TenderOps_API.Models;
using TenderOps_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace TenderOps_API.Repository
{
    public class TenderCategoryRepository : Repository<TenderCategory>, ITenderCategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public TenderCategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<TenderCategory> UpdateAsync(TenderCategory entity)
        {
            _db.TenderCategories.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }




    }
}
