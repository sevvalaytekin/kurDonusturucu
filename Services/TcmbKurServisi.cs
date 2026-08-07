using System.Xml.Linq;
using TcmbKurDonusturucu.Models;
using TcmbKurDonusturucu.Data;
using Microsoft.EntityFrameworkCore;

namespace TcmbKurDonusturucu.Services
{
    public class TcmbKurServisi : ITcmbKurServisi
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;

        public TcmbKurServisi(
            HttpClient httpClient,
            AppDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<Dictionary<string, DovizKuru>> KurlariGetirAsync(DateTime tarih)
        {
            // 1. Önce veritabanında bu tarihe ait kurlar var mı kontrol et
            var dbKurlari = await _dbContext.DovizKurlari
                .Where(x => x.Tarih.Date == tarih.Date)
                .ToListAsync();

            // 2. Varsa TCMB'ye gitmeden veritabanındaki kurları döndür
            if (dbKurlari.Any())
            {
                return dbKurlari.ToDictionary(
                    x => x.Kod,
                    x => x,
                    StringComparer.OrdinalIgnoreCase);
            }

            // 3. Veritabanında yoksa TCMB'ye git
            var url = $"https://www.tcmb.gov.tr/kurlar/{tarih:yyyyMM}/{tarih:ddMMyyyy}.xml";

            var xmlString = await _httpClient.GetStringAsync(url);
            var xdoc = XDocument.Parse(xmlString);

            var sonuc = new Dictionary<string, DovizKuru>(
                StringComparer.OrdinalIgnoreCase);

            // 4. TCMB'den gelen kurları oku
            foreach (var currency in xdoc.Descendants("Currency"))
            {
                var kod = currency.Attribute("CurrencyCode")?.Value ?? "";

                if (string.IsNullOrWhiteSpace(kod))
                    continue;

                decimal.TryParse(
                    currency.Element("Unit")?.Value,
                    out var birim);

                decimal.TryParse(
                    currency.Element("ForexBuying")?.Value,
                    out var alis);

                decimal.TryParse(
                    currency.Element("ForexSelling")?.Value,
                    out var satis);

                var dovizKuru = new DovizKuru
                {
                    Kod = kod,
                    Isim = currency.Element("CurrencyName")?.Value ?? kod,
                    Tarih = tarih,
                    Birim = birim == 0 ? 1 : birim,
                    ForexAlis = alis,
                    ForexSatis = satis
                };

                sonuc[kod] = dovizKuru;

                // 5. TCMB'den aldığımız kuru veritabanına ekle
                _dbContext.DovizKurlari.Add(dovizKuru);
            }

            // 6. Bütün kurları PostgreSQL'e kaydet
            await _dbContext.SaveChangesAsync();

            // 7. Kurları döndür
            return sonuc;
        }
    }
}