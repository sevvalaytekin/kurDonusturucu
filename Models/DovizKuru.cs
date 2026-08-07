using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TcmbKurDonusturucu.Models
{
    [Table("DovizKurlari")]
    public class DovizKuru
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Tarih { get; set; }

        [Required]
        [MaxLength(10)]
        public string DovizKodu { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,4)")]
        public decimal SatisKuru { get; set; }

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    }
}