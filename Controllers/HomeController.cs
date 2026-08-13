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

                var (basarili, birimKur, sonuc) = CaprazKurHesaplayici.Hesapla(kurlar, kaynakDoviz, hedefDoviz, miktar);

                if (!basarili)
                {
                    return Json(new KurHesaplaSonucu
                    {
                        Success = false,
                        Message = "Seçilen tarih için kur bilgisi bulunamadı."
                    });
                }

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
    }
}