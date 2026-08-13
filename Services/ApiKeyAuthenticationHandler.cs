using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace TcmbKurDonusturucu.Services
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string HeaderName = "X-Api-Key";
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration)
            : base(options, logger, encoder)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderName, out var verilenAnahtar))
            {
                return Task.FromResult(AuthenticateResult.Fail("X-Api-Key header eksik."));
            }

            var beklenenAnahtar = _configuration["ApiKey"];

            if (string.IsNullOrEmpty(beklenenAnahtar) || !AnahtarlarEsit(verilenAnahtar.ToString(), beklenenAnahtar))
            {
                return Task.FromResult(AuthenticateResult.Fail("Geçersiz API anahtarı."));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "ApiClient") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private static bool AnahtarlarEsit(string verilen, string beklenen)
        {
            var verilenBytes = Encoding.UTF8.GetBytes(verilen);
            var beklenenBytes = Encoding.UTF8.GetBytes(beklenen);

            return verilenBytes.Length == beklenenBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(verilenBytes, beklenenBytes);
        }
    }
}
