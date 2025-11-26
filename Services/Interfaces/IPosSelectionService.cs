using PosSelection.Models.Pos;

namespace PosSelection.Services.Interfaces;

public interface IPosSelectionService
{
    Task<PosSelectionResponse> SelectBestPosAsync(PosSelectionRequest request, CancellationToken cancellationToken = default);
}

