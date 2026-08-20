using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcmbKurDonusturucu.Services;

namespace TcmbKurDonusturucu.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    public class DonusturController : ControllerBase
    {
        private readonly ITcmbKurServisi _kurServisi;

        public DonusturController(ITcmbKurServisi kurServisi)
        {
            _kurServisi = kurServisi;
        }

        [HttpGet("donustur")]
        public async Task<IActionResult> Get(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] decimal amount,
            [FromQuery] string? date = null)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || amount <= 0)
            {
                return BadRequest(new { message = "'from', 'to' ve pozitif 'amount' zorunludur." });
            }

            DateTime tarih;
            if (string.IsNullOrWhiteSpace(date))
            {
                tarih = DateTime.UtcNow.Date;
            }
            else if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih))
            {
                return BadRequest(new { message = "Tarih formatı geçersiz. Beklenen format: yyyy-MM-dd" });
            }

            try
            {
                var kurlar = await _kurServisi.KurlariGetirAsync(tarih);

                if (kurlar.Count == 0)
                {
                    return NotFound(new { message = "Seçilen tarih için kur bilgisi bulunamadı." });
                }

                var (basarili, birimKur, sonuc) = CaprazKurHesaplayici.Hesapla(kurlar, from, to, amount);

                if (!basarili)
                {
                    return BadRequest(new { message = "Geçersiz döviz kodu." });
                }

                return Ok(new
                {
                    from,
                    to,
                    amount,
                    result = sonuc,
                    rateUsed = birimKur
                });
            }
            catch (HttpRequestException)
            {
                return NotFound(new { message = "Seçilen tarih için kur bilgisi bulunamadı (tatil olabilir)." });
            }
        }
    }
}
