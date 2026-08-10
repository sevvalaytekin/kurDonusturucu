using Microsoft.EntityFrameworkCore;
using TcmbKurDonusturucu.Models;

namespace TcmbKurDonusturucu.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<DovizKuru> DovizKurlari { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kullanici>()
                .HasIndex(x => x.KullaniciAdi)
                .IsUnique();
        }
    }
}