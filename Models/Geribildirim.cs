using System.ComponentModel.DataAnnotations;

namespace TcmbKurDonusturucu.Models
{
    public class Geribildirim
    {
        public int Id { get; set; }

        public string? Ad { get; set; }
        public string Mesaj { get; set; } = string.Empty;
        public DateTime GonderimTarihi { get; set; }
    }

    public class GeribildirimGonderRequest
    {
        [MaxLength(200)]
        public string? Ad { get; set; }

        [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
        [MaxLength(2000)]
        public string Mesaj { get; set; } = string.Empty;
    }
}
