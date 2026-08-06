using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using TcmbKurDonusturucu.Data;
using TcmbKurDonusturucu.Models;

namespace TcmbKurDonusturucu.Controllers
{
    public class KurHesaplaRequest
    {
        public DateTime? Tarih { get; set; }
        public string KaynakDoviz { get; set; } = string.Empty;
        public string HedefDoviz { get; set; } = string.Empty;
        public decimal Miktar { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KurHesapla([FromBody] KurHesaplaRequest model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Geçersiz istek gövdesi." });
            }

            try
            {
                DateTime simdi = DateTime.Now;
                DateTime hesaplamaTarihi = model.Tarih ?? simdi.Date;

                // 15:30 kuralı: Bugünün tarihi girilmişse ve saat 15:30'dan önceyse dünkü kura çek
                if (hesaplamaTarihi.Date == simdi.Date && simdi.TimeOfDay < new TimeSpan(15, 30, 0))
                {
                    hesaplamaTarihi = hesaplamaTarihi.AddDays(-1);
                }

                // Hafta sonu kontrolü
                if (hesaplamaTarihi.DayOfWeek == DayOfWeek.Saturday)
                {
                    hesaplamaTarihi = hesaplamaTarihi.AddDays(-1);
                }
                else if (hesaplamaTarihi.DayOfWeek == DayOfWeek.Sunday)
                {
                    hesaplamaTarihi = hesaplamaTarihi.AddDays(-2);
                }

                DateTime hedefTarih = DateTime.SpecifyKind(hesaplamaTarihi.Date, DateTimeKind.Utc);

                // 1. Önce Veritabanından Oku
                var dbKurlari = await _context.DovizKurlari
                    .Where(k => k.Tarih == hedefTarih)
                    .ToDictionaryAsync(k => k.DovizKodu, k => k.SatisKuru);

                // 2. Veritabanında yoksa TCMB'den Çek ve Kaydet
                if (!dbKurlari.Any())
                {
                    dbKurlari = await TcmbKurlariniCekVeKaydetAsync(hedefTarih, simdi.Date);
                }

                dbKurlari["TRY"] = 1.0m;

                if (!dbKurlari.TryGetValue(model.KaynakDoviz, out decimal kaynakKurTL) ||
                    !dbKurlari.TryGetValue(model.HedefDoviz, out decimal hedefKurTL) ||
                    kaynakKurTL == 0 || hedefKurTL == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Seçilen döviz türlerinden biri bültende bulunamadı."
                    });
                }

                decimal toplamTL = model.Miktar * kaynakKurTL;
                decimal nihaiSonuc = toplamTL / hedefKurTL;
                decimal birimCaprazKur = kaynakKurTL / hedefKurTL;

                return Json(new
                {
                    success = true,
                    tarih = hedefTarih.ToString("dd.MM.yyyy"),
                    kaynak = model.KaynakDoviz,
                    hedef = model.HedefDoviz,
                    girilenMiktar = model.Miktar,
                    birimKur = birimCaprazKur,
                    sonuc = nihaiSonuc
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "Seçilen tarihe ait TCMB verisi alınamadı."
                });
            }
        }

        private async Task<Dictionary<string, decimal>> TcmbKurlariniCekVeKaydetAsync(DateTime hedefTarih, DateTime bugun)
        {
            var kurlar = new Dictionary<string, decimal>();
            string url;

            if (hedefTarih.Date == bugun)
            {
                url = "https://www.tcmb.gov.tr/kurlar/today.xml";
            }
            else
            {
                url = $"https://www.tcmb.gov.tr/kurlar/{hedefTarih:yyyyMM}/{hedefTarih:ddMMyyyy}.xml";
            }

            using (var httpClient = new HttpClient())
            {
                var xmlString = await httpClient.GetStringAsync(url);
                var xdoc = XDocument.Parse(xmlString);

                var eklenecekler = new List<DovizKuru>();

                foreach (var element in xdoc.Descendants("Currency"))
                {
                    string kod = element.Attribute("CurrencyCode")?.Value ?? string.Empty;
                    string satisStr = element.Element("ForexSelling")?.Value;

                    if (!string.IsNullOrEmpty(kod) && !string.IsNullOrEmpty(satisStr) && decimal.TryParse(satisStr.Replace('.', ','), out decimal satisKuru))
                    {
                        kurlar[kod] = satisKuru;

                        eklenecekler.Add(new DovizKuru
                        {
                            Tarih = hedefTarih,
                            DovizKodu = kod,
                            SatisKuru = satisKuru
                        });
                    }
                }

                if (eklenecekler.Any())
                {
                    await _context.DovizKurlari.AddRangeAsync(eklenecekler);
                    await _context.SaveChangesAsync();
                }
            }

            return kurlar;
        }
    }
}