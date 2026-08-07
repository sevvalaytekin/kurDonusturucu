using TcmbKurDonusturucu.Models;
using TcmbKurDonusturucu.Services;
using Microsoft.AspNetCore.Mvc;

namespace TcmbKurDonusturucu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITcmbKurServisi _kurServisi;

        public HomeController(ITcmbKurServisi kurServisi)
        {
            _kurServisi = kurServisi;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KurHesapla(DateTime tarih, string kaynakDoviz, string hedefDoviz, decimal miktar)
        {
            try
            {
                var kurlar = await _kurServisi.KurlariGetirAsync(tarih);

                decimal kaynakTlKarsiligi = KurBul(kurlar, kaynakDoviz);
                decimal hedefTlKarsiligi = KurBul(kurlar, hedefDoviz);

                if (kaynakTlKarsiligi == 0 || hedefTlKarsiligi == 0)
                {
                    return Json(new KurHesaplaSonucu
                    {
                        Success = false,
                        Message = "Seçilen tarih için kur bilgisi bulunamadı."
                    });
                }

                decimal birimKur = kaynakTlKarsiligi / hedefTlKarsiligi;
                decimal sonuc = miktar * birimKur;

                return Json(new KurHesaplaSonucu
                {
                    Success = true,
                    Kaynak = kaynakDoviz,
                    Hedef = hedefDoviz,
                    BirimKur = birimKur,
                    GirilenMiktar = miktar,
                    Sonuc = sonuc
                });
            }
            catch (Exception ex)
            {
                return Json(new KurHesaplaSonucu
                {
                    Success = false,
                    Message = "Kur bilgisi alınırken bir hata oluştu: " + ex.Message
                });
            }
        }

        private decimal KurBul(Dictionary<string, DovizKuru> kurlar, string kod)
        {
            if (kod.Equals("TRY", StringComparison.OrdinalIgnoreCase))
                return 1m;

            if (kurlar.TryGetValue(kod, out var kur))
                return kur.ForexSatis / kur.Birim;

            return 0;
        }
    }
}