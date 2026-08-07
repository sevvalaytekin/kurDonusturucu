using TcmbKurDonusturucu.Models;

namespace TcmbKurDonusturucu.Services
{
    public interface ITcmbKurServisi
    {
        Task<Dictionary<string, DovizKuru>> KurlariGetirAsync(DateTime tarih);
    }
}