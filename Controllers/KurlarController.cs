using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcmbKurDonusturucu.Services;

namespace TcmbKurDonusturucu.Controllers
{
    [ApiController]
    [Route("api/kurlar")]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    public class KurlarController : ControllerBase
    {
        private readonly ITcmbKurServisi _kurServisi;

        public KurlarController(ITcmbKurServisi kurServisi)
        {
            _kurServisi = kurServisi;
        }

        [HttpGet("{tarih}")]
        public async Task<IActionResult> GetKurlar(string tarih)
        {
            if (!DateTime.TryParseExact(tarih, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTarih))
            {
                return BadRequest(new { message = "Tarih formatı geçersiz. Beklenen format: yyyy-MM-dd" });
            }

            try
            {
                var kurlar = await _kurServisi.KurlariGetirAsync(parsedTarih);

                if (kurlar.Count == 0)
                {
                    return NotFound(new { message = "Seçilen tarih için kur bilgisi bulunamadı." });
                }

                return Ok(kurlar);
            }
            catch (HttpRequestException)
            {
                return NotFound(new { message = "Seçilen tarih için kur bilgisi bulunamadı (tatil olabilir)." });
            }
        }
    }
}
