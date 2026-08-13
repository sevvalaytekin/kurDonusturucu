using System.Globalization;
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
            // Postgres "timestamptz" kolonu yalnızca Kind=Utc kabul ediyor;
            // çağıranın gönderdiği DateTime'ın Kind'ı ne olursa olsun burada normalize ediyoruz.
            tarih = DateTime.SpecifyKind(tarih.Date, DateTimeKind.Utc);

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
            var sonuc = XmlAyristir(xmlString, tarih);

            // 5. TCMB'den ayrıştırılan kurları veritabanına ekle
            foreach (var dovizKuru in sonuc.Values)
            {
                _dbContext.DovizKurlari.Add(dovizKuru);
            }

            // 6. Bütün kurları PostgreSQL'e kaydet
            await _dbContext.SaveChangesAsync();

            // 7. Kurları döndür
            return sonuc;
        }

        internal static Dictionary<string, DovizKuru> XmlAyristir(string xmlString, DateTime tarih)
        {
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
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var birim);

                decimal.TryParse(
                    currency.Element("ForexBuying")?.Value,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var alis);

                decimal.TryParse(
                    currency.Element("ForexSelling")?.Value,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
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
            }

            return sonuc;
        }
    }
}