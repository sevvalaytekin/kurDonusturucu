namespace TcmbKurDonusturucu.Models
{
    public class DovizKuru
    {
        public int Id { get; set; }

        public string Kod { get; set; } = string.Empty;
        public string Isim { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
        public decimal Birim { get; set; } = 1;
        public decimal ForexAlis { get; set; }
        public decimal ForexSatis { get; set; }
    }

    public class KurHesaplaSonucu
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string Kaynak { get; set; } = string.Empty;
        public string Hedef { get; set; } = string.Empty;
        public decimal BirimKur { get; set; }
        public decimal GirilenMiktar { get; set; }
        public decimal Sonuc { get; set; }
    }
}