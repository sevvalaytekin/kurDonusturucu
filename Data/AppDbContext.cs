using Microsoft.EntityFrameworkCore;
using TcmbKurDonusturucu.Models;

namespace TcmbKurDonusturucu.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<DovizKuru> DovizKurlari { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DovizKuru>()
                .HasIndex(d => new { d.Tarih, d.DovizKodu })
                .IsUnique();
        }
    }
}