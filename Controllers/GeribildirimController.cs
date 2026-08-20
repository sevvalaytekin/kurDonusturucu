using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcmbKurDonusturucu.Models;
using TcmbKurDonusturucu.Services;

namespace TcmbKurDonusturucu.Controllers
{
    [ApiController]
    [Route("api/geribildirim")]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    public class GeribildirimController : ControllerBase
    {
        private readonly IGeribildirimServisi _geribildirimServisi;

        public GeribildirimController(IGeribildirimServisi geribildirimServisi)
        {
            _geribildirimServisi = geribildirimServisi;
        }

        [HttpPost]
        public async Task<IActionResult> Gonder([FromBody] GeribildirimGonderRequest request)
        {
            var geribildirim = await _geribildirimServisi.KaydetAsync(request.Ad, request.Mesaj);
            return StatusCode(StatusCodes.Status201Created, geribildirim);
        }
    }
}
