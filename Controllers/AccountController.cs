using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

            var dogrulamaBasarili = kullanici != null &&
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
    }
}
