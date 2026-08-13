using TcmbKurDonusturucu.Models;

namespace TcmbKurDonusturucu.Services
{
    internal static class CaprazKurHesaplayici
    {
        internal static decimal TlKarsiligiBul(Dictionary<string, DovizKuru> kurlar, string kod)
        {
            if (kod.Equals("TRY", StringComparison.OrdinalIgnoreCase))
                return 1m;

            if (kurlar.TryGetValue(kod, out var kur))
                return kur.ForexSatis / kur.Birim;

            return 0;
        }

        internal static (bool basarili, decimal birimKur, decimal sonuc) Hesapla(
            Dictionary<string, DovizKuru> kurlar, string kaynakDoviz, string hedefDoviz, decimal miktar)
        {
            var kaynakTl = TlKarsiligiBul(kurlar, kaynakDoviz);
            var hedefTl = TlKarsiligiBul(kurlar, hedefDoviz);

            if (kaynakTl == 0 || hedefTl == 0)
                return (false, 0, 0);

            var birimKur = kaynakTl / hedefTl;
            return (true, birimKur, miktar * birimKur);
        }
    }
}
