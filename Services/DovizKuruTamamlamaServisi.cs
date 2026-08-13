namespace TcmbKurDonusturucu.Services
{
    public class DovizKuruTamamlamaServisi : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DovizKuruTamamlamaServisi> _logger;

        public DovizKuruTamamlamaServisi(
            IServiceScopeFactory scopeFactory,
            ILogger<DovizKuruTamamlamaServisi> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var kurServisi = scope.ServiceProvider.GetRequiredService<ITcmbKurServisi>();

            _logger.LogInformation("Son 30 gunun doviz kuru tamamlama islemi basladi.");

            for (int i = 0; i < 30 && !stoppingToken.IsCancellationRequested; i++)
            {
                var tarih = DateTime.Today.AddDays(-i);

                if (tarih.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                    continue;

                try
                {
                    await kurServisi.KurlariGetirAsync(tarih);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "{Tarih} icin kur verisi alinamadi (tatil olabilir).", tarih.ToShortDateString());
                }

                await Task.Delay(200, stoppingToken);
            }

            _logger.LogInformation("Son 30 gunun doviz kuru tamamlama islemi bitti.");
        }
    }
}
