using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Xml;

namespace TcmbConverter.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KurHesapla(
            DateTime? tarih,
            string kaynakDoviz,
            string hedefDoviz,
            decimal miktar)
        {
            try
            {
                DateTime simdi = DateTime.Now;
                DateTime hesaplamaTarihi = tarih ?? simdi.Date;

                /*
                 * TCMB, günün gösterge niteliğindeki döviz kurlarını
                 * iş günlerinde saat 15:30'da yayımlar.
                 *
                 * Kullanıcı bugünü seçmişse ve saat henüz 15:30 olmamışsa
                 * bir önceki günün kuru kullanılmalıdır.
                 */
                if (hesaplamaTarihi.Date == simdi.Date &&
                    simdi.TimeOfDay < new TimeSpan(15, 30, 0))
                {
                    hesaplamaTarihi = hesaplamaTarihi.AddDays(-1);
                }

                /*
                 * Tarih hafta sonuna denk geliyorsa cuma gününe dönülür.
                 *
                 * Pazartesi 15:30'dan önce:
                 * Önce pazar gününe gidilir, ardından cuma gününe çekilir.
                 */
                if (hesaplamaTarihi.DayOfWeek == DayOfWeek.Saturday)
                {
                    hesaplamaTarihi = hesaplamaTarihi.AddDays(-1);
                }
                else if (hesaplamaTarihi.DayOfWeek == DayOfWeek.Sunday)
                {
                    hesaplamaTarihi = hesaplamaTarihi.AddDays(-2);
                }

                string url;

                /*
                 * Bugünün yayımlanmış kuru isteniyorsa today.xml,
                 * geçmiş tarih isteniyorsa arşiv adresi kullanılır.
                 */
                if (hesaplamaTarihi.Date == simdi.Date)
                {
                    url = "https://www.tcmb.gov.tr/kurlar/today.xml";
                }
                else
                {
                    string yilAy = hesaplamaTarihi.ToString("yyyyMM");
                    string gunAyYil = hesaplamaTarihi.ToString("ddMMyyyy");

                    url =
                        $"https://www.tcmb.gov.tr/kurlar/{yilAy}/{gunAyYil}.xml";
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(url);

                decimal kaynakKurTL =
                    GetKurInTL(xmlDoc, kaynakDoviz);

                decimal hedefKurTL =
                    GetKurInTL(xmlDoc, hedefDoviz);

                if (kaynakKurTL == 0 || hedefKurTL == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Seçilen döviz türlerinden biri TCMB bülteninde bulunamadı."
                    });
                }

                decimal toplamTL = miktar * kaynakKurTL;
                decimal nihaiSonuc = toplamTL / hedefKurTL;
                decimal birimCaprazKur = kaynakKurTL / hedefKurTL;

                return Json(new
                {
                    success = true,
                    tarih = hesaplamaTarihi.ToString("dd.MM.yyyy"),
                    kaynak = kaynakDoviz,
                    hedef = hedefDoviz,
                    girilenMiktar = miktar,
                    birimKur = birimCaprazKur,
                    sonuc = nihaiSonuc
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "Seçilen tarihe ait TCMB verisi alınamadı. Resmî tatil veya servis hatası olabilir."
                });
            }
        }

        private decimal GetKurInTL(
            XmlDocument doc,
            string dovizKodu)
        {
            if (dovizKodu == "TRY")
            {
                return 1.0m;
            }

            XmlNode? node =
                doc.SelectSingleNode(
                    $"//Currency[@Kod='{dovizKodu}']");

            if (node == null)
            {
                return 0m;
            }

            string forexSelling =
                node["ForexSelling"]?.InnerText ?? "0";

            if (decimal.TryParse(
                    forexSelling,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out decimal kur))
            {
                return kur;
            }

            return 0m;
        }
    }
}