using PosSelection.Services.Interfaces;
using System.Globalization;

namespace PosSelection.Services.Background;

public class RatioRefreshBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RatioRefreshBackgroundService> _logger;
    private readonly TimeZoneInfo _istanbulTimeZone;

    public RatioRefreshBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RatioRefreshBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        try
        {
            _istanbulTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        }
        catch
        {
            try
            {
                _istanbulTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
            }
            catch
            {
                _logger.LogWarning("Istanbul timezone bulunamadı, UTC kullanılacak");
                _istanbulTimeZone = TimeZoneInfo.Utc;
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _istanbulTimeZone);
                var nextRefresh = GetNextRefreshTime(now);

                var delay = nextRefresh - now;
                _logger.LogInformation(
                    "Bir sonraki oran güncellemesi: {NextRefresh} (şu an: {Now}, bekleme: {Delay})",
                    nextRefresh, now, delay);

                if (delay.TotalMilliseconds > 0)
                {
                    await Task.Delay(delay, stoppingToken);
                }

                await RefreshRatiosAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Oran güncelleme sırasında hata oluştu");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }

    private DateTime GetNextRefreshTime(DateTime now)
    {
        var today = now.Date;
        var refreshTime = today.AddHours(23).AddMinutes(59);

        if (now >= refreshTime)
        {
            refreshTime = refreshTime.AddDays(1);
        }

        return refreshTime;
    }

    private async Task RefreshRatiosAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var ratioService = scope.ServiceProvider.GetRequiredService<IRatioService>();

        try
        {
            await ratioService.RefreshRatiosAsync(cancellationToken);
            _logger.LogInformation("Oranlar başarıyla güncellendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oran güncelleme başarısız oldu");
        }
    }
}

