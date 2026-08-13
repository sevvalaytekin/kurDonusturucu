using TcmbKurDonusturucu.Models;
using TcmbKurDonusturucu.Services;

namespace TcmbKurDonusturucu.Tests;

public class CaprazKurHesaplayiciTests
{
    private static Dictionary<string, DovizKuru> OrnekKurlar() =>
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["USD"] = new DovizKuru { Kod = "USD", Birim = 1, ForexSatis = 40m },
            ["EUR"] = new DovizKuru { Kod = "EUR", Birim = 1, ForexSatis = 44m },
            ["JPY"] = new DovizKuru { Kod = "JPY", Birim = 100, ForexSatis = 27m },
        };

    [Fact]
    public void TryDenTryYe_BirimKurBirSonucMiktardir()
    {
        var (basarili, birimKur, sonuc) = CaprazKurHesaplayici.Hesapla(OrnekKurlar(), "TRY", "TRY", 250m);

        Assert.True(basarili);
        Assert.Equal(1m, birimKur);
        Assert.Equal(250m, sonuc);
    }

    [Fact]
    public void UsdDenTryYe_DogruHesaplanir()
    {
        var (basarili, birimKur, sonuc) = CaprazKurHesaplayici.Hesapla(OrnekKurlar(), "USD", "TRY", 100m);

        Assert.True(basarili);
        Assert.Equal(40m, birimKur);
        Assert.Equal(4000m, sonuc);
    }

    [Fact]
    public void UsdDenEuroya_CaprazKurDogruHesaplanir()
    {
        // Elle hesap: USD TL karşılığı = 40, EUR TL karşılığı = 44 -> birimKur = 40/44
        var (basarili, birimKur, sonuc) = CaprazKurHesaplayici.Hesapla(OrnekKurlar(), "USD", "EUR", 100m);

        Assert.True(basarili);
        Assert.Equal(40m / 44m, birimKur);
        Assert.Equal(100m * (40m / 44m), sonuc);
    }

    [Fact]
    public void BilinmeyenDovizKodu_BasarisizDoner()
    {
        var (basarili, birimKur, sonuc) = CaprazKurHesaplayici.Hesapla(OrnekKurlar(), "XYZ", "TRY", 100m);

        Assert.False(basarili);
        Assert.Equal(0m, birimKur);
        Assert.Equal(0m, sonuc);
    }

    [Fact]
    public void BirimBirdenFarkliDoviz_BolmeDogruYapilir()
    {
        // JPY: ForexSatis=27, Birim=100 -> TL karşılığı = 27/100 = 0.27
        var (basarili, birimKur, _) = CaprazKurHesaplayici.Hesapla(OrnekKurlar(), "JPY", "TRY", 1000m);

        Assert.True(basarili);
        Assert.Equal(0.27m, birimKur);
    }

    [Fact]
    public void TlKarsiligiBul_TryIcinHerZamanBirDoner()
    {
        var sonuc = CaprazKurHesaplayici.TlKarsiligiBul(OrnekKurlar(), "try");

        Assert.Equal(1m, sonuc);
    }
}
