using Microsoft.AspNetCore.Mvc;
using System.Xml; // XML işlemleri için şart

namespace TcmbConverter.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // JavaScript'in (Fetch API) istek atacağı metod
        [HttpPost]
        public IActionResult KurHesapla(DateTime? tarih, string kaynakDoviz, string hedefDoviz, decimal miktar)
        {
            try
            {
                DateTime hesaplamaTarihi = tarih ?? DateTime.Today;

                // 1. Hafta Sonu Kontrolü (TCMB hafta sonu veri yayınlamaz)
                if (hesaplamaTarihi.DayOfWeek == DayOfWeek.Saturday)
                    hesaplamaTarihi = hesaplamaTarihi.AddDays(-1);
                else if (hesaplamaTarihi.DayOfWeek == DayOfWeek.Sunday)
                    hesaplamaTarihi = hesaplamaTarihi.AddDays(-2);

                // 2. TCMB URL Oluşturma
                string url;
                if (hesaplamaTarihi.Date == DateTime.Today)
                {
                    url = "https://www.tcmb.gov.tr/kurlar/today.xml";
                }
                else
                {
                    string yilAy = hesaplamaTarihi.ToString("yyyyMM");
                    string gunAyYil = hesaplamaTarihi.ToString("ddMMyyyy");
                    url = $"https://www.tcmb.gov.tr/kurlar/{yilAy}/{gunAyYil}.xml";
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(url);

                // 3. Kurları TL Cinsinden Elde Etme
                decimal kaynakKurTL = GetKurInTL(xmlDoc, kaynakDoviz);
                decimal hedefKurTL = GetKurInTL(xmlDoc, hedefDoviz);

                if (kaynakKurTL == 0 || hedefKurTL == 0)
                {
                    return Json(new { success = false, message = "Seçilen döviz türlerinden biri TCMB bülteninde bulunamadı." });
                }

                // 4. Çapraz Dönüşüm Hesabı
                decimal toplamTL = miktar * kaynakKurTL;
                decimal nihaiSonuc = toplamTL / hedefKurTL;
                decimal birimCaprazKur = kaynakKurTL / hedefKurTL;

                // 5. Sonucu JavaScript'e JSON olarak döndürme
                return Json(new
                {
                    success = true,
                    kaynak = kaynakDoviz,
                    hedef = hedefDoviz,
                    girilenMiktar = miktar,
                    birimKur = birimCaprazKur,
                    sonuc = nihaiSonuc
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Seçilen tarihe ait TCMB verisi alınamadı (Resmi tatil veya servis hatası olabilir)." });
            }
        }

        // Yardımcı Metod: XML'den Kuru Okur
        private decimal GetKurInTL(XmlDocument doc, string dovizKodu)
        {
            if (dovizKodu == "TRY") return 1.0m; 

            XmlNode? node = doc.SelectSingleNode($"//Currency[@Kod='{dovizKodu}']");
            if (node != null)
            {
                string forexSelling = node["ForexSelling"]?.InnerText ?? "0";
                return decimal.Parse(forexSelling, System.Globalization.CultureInfo.InvariantCulture);
            }

            return 0m;
        }
    }
}