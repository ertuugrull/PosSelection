using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using PosSelection.Models.Ration;
using PosSelection.Services.Interfaces;

namespace PosSelection.Services;

public class RatioService(HttpClient httpClient, ILogger<RatioService> logger, IMemoryCache memoryCache) : IRatioService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<RatioService> _logger = logger;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private const string CacheKey = "PayTr_Ratios";

    public async Task<List<Ratio>> GetAllRatiosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("paytr/ratios", cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiRatios = JsonConvert.DeserializeObject<List<RatioApiResponse>>(
                jsonContent,
                new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include
                }
            );

            if (apiRatios == null)
            {
                _logger.LogWarning("POS servisten boş veri döndü");
                return new List<Ratio>();
            }

            var ratios = apiRatios.Select(r => new Ratio
            {
                PosName = r.PosName,
                CardType = r.CardType,
                CardBrand = r.CardBrand,
                Installment = r.Installment,
                Currency = r.Currency,
                CommissionRate = r.CommissionRate,
                MinFee = r.MinFee,
                Priority = r.Priority
            }).ToList();

            _logger.LogInformation("{Count} adet oran başarıyla çekildi", ratios.Count);
            return ratios;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oranlar çekilirken hata oluştu");
            throw;
        }
    }

    public async Task RefreshRatiosAsync(CancellationToken cancellationToken = default)
    {
        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            _logger.LogInformation("Oranlar güncelleniyor...");
            var ratios = await GetAllRatiosAsync(cancellationToken);
            
            var cacheOptions = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove
            };
            
            _memoryCache.Set(CacheKey, ratios, cacheOptions);

            _logger.LogInformation("Oranlar başarıyla güncellendi ve cache'e kaydedildi. Toplam: {Count}", ratios.Count);
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<List<Ratio>> GetCachedRatiosAsync(CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue<List<Ratio>>(CacheKey, out var cachedRatios))
        {
            return cachedRatios ?? new List<Ratio>();
        }
         // distrubuted cache olmadığı için cache kaybolma tehlikesi mevcvut.
        _logger.LogInformation("Cache'te oran bulunamadı, POS servisten çekiliyor...");
        try
        {
            await RefreshRatiosAsync(cancellationToken);
            
            if (_memoryCache.TryGetValue<List<Ratio>>(CacheKey, out var refreshedRatios))
            {
                _logger.LogInformation("Oranlar POS servisten çekildi ve cache'e kaydedildi. Toplam: {Count}", refreshedRatios?.Count ?? 0);
                return refreshedRatios ?? new List<Ratio>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache'te oran yokken POS servisten çekme başarısız oldu");
        }
        
        return new List<Ratio>();
    }
}

