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
    }
}