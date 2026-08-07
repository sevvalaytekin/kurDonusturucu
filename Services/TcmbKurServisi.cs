using System.Xml.Linq;
using TcmbKurDonusturucu.Models;

namespace TcmbKurDonusturucu.Services
{
    public class TcmbKurServisi : ITcmbKurServisi
    {
        private readonly HttpClient _httpClient;

        public TcmbKurServisi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, DovizKuru>> KurlariGetirAsync(DateTime tarih)
        {
            var url = $"https://www.tcmb.gov.tr/kurlar/{tarih:yyyyMM}/{tarih:ddMMyyyy}.xml";
            var xmlString = await _httpClient.GetStringAsync(url);
            var xdoc = XDocument.Parse(xmlString);

            var sonuc = new Dictionary<string, DovizKuru>(StringComparer.OrdinalIgnoreCase);

            foreach (var currency in xdoc.Descendants("Currency"))
            {
                var kod = currency.Attribute("CurrencyCode")?.Value ?? "";
                if (string.IsNullOrWhiteSpace(kod)) continue;

                decimal.TryParse(currency.Element("Unit")?.Value, out var birim);
                decimal.TryParse(currency.Element("ForexBuying")?.Value, out var alis);
                decimal.TryParse(currency.Element("ForexSelling")?.Value, out var satis);

                sonuc[kod] = new DovizKuru
                {
                    Kod = kod,
                    Isim = currency.Element("CurrencyName")?.Value ?? kod,
                    Birim = birim == 0 ? 1 : birim,
                    ForexAlis = alis,
                    ForexSatis = satis
                };
            }

            return sonuc;
        }
    }
}