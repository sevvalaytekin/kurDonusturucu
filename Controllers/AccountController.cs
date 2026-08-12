using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TcmbKurDonusturucu.Data;
using TcmbKurDonusturucu.Models;

namespace TcmbKurDonusturucu.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly PasswordHasher<Kullanici> _passwordHasher = new();

        public AccountController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string kullaniciAdi, string sifre)
        {
            var kullanici = await _dbContext.Kullanicilar
                .FirstOrDefaultAsync(x => x.KullaniciAdi == kullaniciAdi);

            var dogrulamaBasarili = kullanici?.SifreHash != null &&
                _passwordHasher.VerifyHashedPassword(kullanici, kullanici.SifreHash, sifre) != PasswordVerificationResult.Failed;

            if (!dogrulamaBasarili)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, kullanici!.KullaniciAdi)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult GoogleGiris()
        {
            var redirectUrl = Url.Action(nameof(GoogleGirisCallback));
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleGirisCallback()
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction(nameof(Login));

            var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
                return RedirectToAction(nameof(Login));

            var kullanici = await _dbContext.Kullanicilar
                .FirstOrDefaultAsync(x => x.GoogleId == googleId);

            if (kullanici == null)
            {
                kullanici = new Kullanici
                {
                    KullaniciAdi = email,
                    GoogleId = googleId,
                    SifreHash = null
                };

                _dbContext.Kullanicilar.Add(kullanici);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
