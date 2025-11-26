using PosSelection.Models.Ration;

namespace PosSelection.Services.Interfaces;

public interface IRatioService
{
    Task<List<Ratio>> GetAllRatiosAsync(CancellationToken cancellationToken = default);
    Task RefreshRatiosAsync(CancellationToken cancellationToken = default);
    Task<List<Ratio>> GetCachedRatiosAsync(CancellationToken cancellationToken = default);
}

