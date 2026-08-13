using TcmbKurDonusturucu.Services;

namespace TcmbKurDonusturucu.Tests;

public class TcmbKurServisiXmlAyristirTests
{
    private static readonly DateTime Tarih = new(2026, 8, 12, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void NoktaOndalikAyracliDegerler_DogruParseEdilir()
    {
        // Regresyon testi: TCMB "47.7537" gibi nokta ondalık ayraçlı değerler gönderiyor.
        // CultureInfo.InvariantCulture kullanılmazsa (örn. tr-TR kültüründe) "." binlik ayraç
        // sanılıp değer "477537" olarak okunuyordu — bu hata gerçekte yaşanmıştı.
        const string xml = """
            <Tarih_Date>
                <Currency CurrencyCode="USD">
                    <Unit>1</Unit>
                    <CurrencyName>US DOLLAR</CurrencyName>
                    <ForexBuying>47.6678</ForexBuying>
                    <ForexSelling>47.7537</ForexSelling>
                </Currency>
            </Tarih_Date>
            """;

        var sonuc = TcmbKurServisi.XmlAyristir(xml, Tarih);

        Assert.Equal(47.6678m, sonuc["USD"].ForexAlis);
        Assert.Equal(47.7537m, sonuc["USD"].ForexSatis);
    }

    [Fact]
    public void BirdenFazlaCurrency_DogruKodlarlaSozlugeGirer()
    {
        const string xml = """
            <Tarih_Date>
                <Currency CurrencyCode="USD">
                    <Unit>1</Unit>
                    <CurrencyName>US DOLLAR</CurrencyName>
                    <ForexBuying>47.6678</ForexBuying>
                    <ForexSelling>47.7537</ForexSelling>
                </Currency>
                <Currency CurrencyCode="EUR">
                    <Unit>1</Unit>
                    <CurrencyName>EURO</CurrencyName>
                    <ForexBuying>52.1000</ForexBuying>
                    <ForexSelling>52.2000</ForexSelling>
                </Currency>
            </Tarih_Date>
            """;

        var sonuc = TcmbKurServisi.XmlAyristir(xml, Tarih);

        Assert.Equal(2, sonuc.Count);
        Assert.True(sonuc.ContainsKey("USD"));
        Assert.True(sonuc.ContainsKey("EUR"));
    }

    [Fact]
    public void UnitEksikVeyaSifirsa_BirimBireFallbackEder()
    {
        const string xml = """
            <Tarih_Date>
                <Currency CurrencyCode="JPY">
                    <Unit>0</Unit>
                    <CurrencyName>JAPENESE YEN</CurrencyName>
                    <ForexBuying>30.0000</ForexBuying>
                    <ForexSelling>30.5000</ForexSelling>
                </Currency>
            </Tarih_Date>
            """;

        var sonuc = TcmbKurServisi.XmlAyristir(xml, Tarih);

        Assert.Equal(1m, sonuc["JPY"].Birim);
    }

    [Fact]
    public void CurrencyCodeBosOlanEleman_Atlanir()
    {
        const string xml = """
            <Tarih_Date>
                <Currency CurrencyCode="">
                    <Unit>1</Unit>
                    <CurrencyName>Bilinmeyen</CurrencyName>
                    <ForexBuying>1.0000</ForexBuying>
                    <ForexSelling>1.1000</ForexSelling>
                </Currency>
                <Currency CurrencyCode="USD">
                    <Unit>1</Unit>
                    <CurrencyName>US DOLLAR</CurrencyName>
                    <ForexBuying>47.6678</ForexBuying>
                    <ForexSelling>47.7537</ForexSelling>
                </Currency>
            </Tarih_Date>
            """;

        var sonuc = TcmbKurServisi.XmlAyristir(xml, Tarih);

        Assert.Single(sonuc);
        Assert.True(sonuc.ContainsKey("USD"));
    }

    [Fact]
    public void Sozluk_BuyukKucukHarfDuyarsizdir()
    {
        const string xml = """
            <Tarih_Date>
                <Currency CurrencyCode="USD">
                    <Unit>1</Unit>
                    <CurrencyName>US DOLLAR</CurrencyName>
                    <ForexBuying>47.6678</ForexBuying>
                    <ForexSelling>47.7537</ForexSelling>
                </Currency>
            </Tarih_Date>
            """;

        var sonuc = TcmbKurServisi.XmlAyristir(xml, Tarih);

        Assert.True(sonuc.ContainsKey("usd"));
    }

    [Fact]
    public void EksikSayisalAlan_ExceptionFirlatmadanSifirOlur()
    {
        const string xml = """
            <Tarih_Date>
                <Currency CurrencyCode="USD">
                    <Unit>1</Unit>
                    <CurrencyName>US DOLLAR</CurrencyName>
                    <ForexSelling>47.7537</ForexSelling>
                </Currency>
            </Tarih_Date>
            """;

        var sonuc = TcmbKurServisi.XmlAyristir(xml, Tarih);

        Assert.Equal(0m, sonuc["USD"].ForexAlis);
        Assert.Equal(47.7537m, sonuc["USD"].ForexSatis);
    }
}
