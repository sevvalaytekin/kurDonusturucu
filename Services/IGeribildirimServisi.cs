using TcmbKurDonusturucu.Models;

namespace TcmbKurDonusturucu.Services
{
    public interface IGeribildirimServisi
    {
        Task<Geribildirim> KaydetAsync(string? ad, string mesaj);
    }
}
