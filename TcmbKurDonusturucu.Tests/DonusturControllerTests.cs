using Microsoft.AspNetCore.Mvc;
using TcmbKurDonusturucu.Controllers;
using TcmbKurDonusturucu.Models;
using TcmbKurDonusturucu.Services;

namespace TcmbKurDonusturucu.Tests;

public class DonusturControllerTests
{
    private class FakeKurServisi : ITcmbKurServisi
    {
        private readonly Dictionary<string, DovizKuru> _kurlar;

        public FakeKurServisi(Dictionary<string, DovizKuru>? kurlar = null)
        {
            _kurlar = kurlar ?? OrnekKurlar();
        }

        public Task<Dictionary<string, DovizKuru>> KurlariGetirAsync(DateTime tarih) =>
            Task.FromResult(_kurlar);
    }

    private static Dictionary<string, DovizKuru> OrnekKurlar() =>
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["USD"] = new DovizKuru { Kod = "USD", Birim = 1, ForexSatis = 40m },
            ["EUR"] = new DovizKuru { Kod = "EUR", Birim = 1, ForexSatis = 44m },
        };

    [Fact]
    public async Task Get_EksikParametre_BadRequestDoner()
    {
        var ctrl = new DonusturController(new FakeKurServisi());

        var result = await ctrl.Get("", "USD", 100m);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Get_SifirVeyaNegatifMiktar_BadRequestDoner()
    {
        var ctrl = new DonusturController(new FakeKurServisi());

        var result = await ctrl.Get("USD", "EUR", 0m);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Get_GecersizTarihFormati_BadRequestDoner()
    {
        var ctrl = new DonusturController(new FakeKurServisi());

        var result = await ctrl.Get("USD", "EUR", 100m, "19.08.2026");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Get_KurBulunamadi_NotFoundDoner()
    {
        var ctrl = new DonusturController(new FakeKurServisi(new Dictionary<string, DovizKuru>()));

        var result = await ctrl.Get("USD", "EUR", 100m, "2026-08-19");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Get_BilinmeyenDovizKodu_BadRequestDoner()
    {
        var ctrl = new DonusturController(new FakeKurServisi());

        var result = await ctrl.Get("XYZ", "EUR", 100m, "2026-08-19");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Get_GecerliIstek_DogruSonucDoner()
    {
        var ctrl = new DonusturController(new FakeKurServisi());

        var result = await ctrl.Get("USD", "EUR", 100m, "2026-08-19") as OkObjectResult;

        Assert.NotNull(result);
        // USD TL karşılığı 40, EUR TL karşılığı 44 -> birimKur = 40/44, sonuc = 100 * 40/44
        var beklenenBirimKur = 40m / 44m;
        var beklenenSonuc = 100m * beklenenBirimKur;

        var deger = result!.Value;
        Assert.Equal(beklenenSonuc, (decimal)deger!.GetType().GetProperty("result")!.GetValue(deger)!);
        Assert.Equal(beklenenBirimKur, (decimal)deger!.GetType().GetProperty("rateUsed")!.GetValue(deger)!);
    }
}
