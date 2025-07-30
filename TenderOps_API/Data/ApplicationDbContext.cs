using TenderOps_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TenderOps_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Tender> Tenders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<TenderCategory> TenderCategories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Partner> Partners { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Invoice>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Tender>()
                .HasOne(t => t.Invoice)
                .WithOne(i => i.Tender)
                .HasForeignKey<Invoice>(i => i.TenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tender>()
                .HasOne(t => t.TenderCategory)
                .WithMany(c => c.Tenders)
                .HasForeignKey(t => t.TenderCategoryId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Tender>()
                .HasOne(t => t.Partner)
                .WithMany(p => p.Tenders)
                .HasForeignKey(t => t.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Partner)
                .WithMany(p => p.ApplicationUsers) // لو One to Many
                .HasForeignKey(u => u.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);




        }
    }
}
