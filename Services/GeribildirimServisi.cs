using TcmbKurDonusturucu.Data;
using TcmbKurDonusturucu.Models;

namespace TcmbKurDonusturucu.Services
{
    public class GeribildirimServisi : IGeribildirimServisi
    {
        private readonly AppDbContext _dbContext;

        public GeribildirimServisi(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Geribildirim> KaydetAsync(string? ad, string mesaj)
        {
            var geribildirim = new Geribildirim
            {
                Ad = NormallestirAd(ad),
                Mesaj = mesaj.Trim(),
                GonderimTarihi = DateTime.UtcNow
            };

            _dbContext.Geribildirimler.Add(geribildirim);
            await _dbContext.SaveChangesAsync();

            return geribildirim;
        }

        internal static string? NormallestirAd(string? ad) =>
            string.IsNullOrWhiteSpace(ad) ? null : ad.Trim();
    }
}
