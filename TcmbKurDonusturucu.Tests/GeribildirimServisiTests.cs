using TcmbKurDonusturucu.Services;

namespace TcmbKurDonusturucu.Tests;

public class GeribildirimServisiTests
{
    [Fact]
    public void BosString_NullDoner()
    {
        var sonuc = GeribildirimServisi.NormallestirAd("");

        Assert.Null(sonuc);
    }

    [Fact]
    public void SadeceBosluk_NullDoner()
    {
        var sonuc = GeribildirimServisi.NormallestirAd("   ");

        Assert.Null(sonuc);
    }

    [Fact]
    public void Null_NullDoner()
    {
        var sonuc = GeribildirimServisi.NormallestirAd(null);

        Assert.Null(sonuc);
    }

    [Fact]
    public void BastanSondanBosluklu_TrimEdilmisHaliDoner()
    {
        var sonuc = GeribildirimServisi.NormallestirAd("  Ayşe  ");

        Assert.Equal("Ayşe", sonuc);
    }

    [Fact]
    public void NormalAd_AyneniDoner()
    {
        var sonuc = GeribildirimServisi.NormallestirAd("Mehmet");

        Assert.Equal("Mehmet", sonuc);
    }
}
