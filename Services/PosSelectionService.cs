using PosSelection.Extensions;
using PosSelection.Helpers;
using PosSelection.Models.Pos;
using PosSelection.Services.Interfaces;

namespace PosSelection.Services;

public class PosSelectionService(IRatioService ratioService, ILogger<PosSelectionService> logger) : IPosSelectionService
{
    private readonly IRatioService _ratioService = ratioService;
    private readonly ILogger<PosSelectionService> _logger = logger;

    public async Task<PosSelectionResponse> SelectBestPosAsync(PosSelectionRequest request, CancellationToken cancellationToken = default)
    {
        var ratios = await _ratioService.GetCachedRatiosAsync(cancellationToken);
        if (ratios.Count == 0)
        {
            _logger.LogWarning("Oranlar yüklenemedi");
            throw new InvalidOperationException("Oranlar yüklenemedi. Lütfen daha sonra tekrar deneyin.");
        }

        var filteredRatios = ratios.Where(r =>
            r.Installment == request.Installment &&
            r.Currency.Equals(request.Currency, StringComparison.OrdinalIgnoreCase) &&
            (string.IsNullOrWhiteSpace(request.CardType) || r.CardType.Equals(request.CardType, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(request.CardBrand) || r.CardBrand.Equals(request.CardBrand, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        if (filteredRatios.Count == 0)
        {
            _logger.LogWarning("Filtrelere uygun POS bulunamadı. Request: {@Request}", request);
            throw new InvalidOperationException("Belirtilen kriterlere uygun POS bulunamadı.");
        }

        var candidates = filteredRatios.Select(ratio =>
        {
            var cost = PosHelper.CalculateCost(request.Amount, ratio.CommissionRate, ratio.MinFee, request.Currency);
            return new
            {
                Ratio = ratio,
                Cost = cost
            };
        }).ToList();

        var bestCandidate = candidates
            .OrderBy(c => c.Cost)
            .ThenByDescending(c => c.Ratio.Priority)
            .ThenBy(c => c.Ratio.CommissionRate)
            .ThenBy(c => c.Ratio.PosName)
            .First();

        var bestRatio = bestCandidate.Ratio;
        var price = bestCandidate.Cost.RoundHalfUp();
        var payableTotal = (request.Amount + price).RoundHalfUp();

        _logger.LogInformation(
            "En uygun POS seçildi: {PosName}, Cost: {Cost}, Price: {Price}",
            bestRatio.PosName, bestCandidate.Cost, price);

        return new PosSelectionResponse
        {
            Filters = new PosSelectionFilters
            {
                Amount = request.Amount,
                Installment = request.Installment,
                Currency = request.Currency,
                CardType = request.CardType,
                CardBrand = request.CardBrand
            },
            OverallMin = new PosSelectionResult
            {
                PosName = bestRatio.PosName,
                CardType = bestRatio.CardType,
                CardBrand = bestRatio.CardBrand,
                Installment = bestRatio.Installment,
                Currency = bestRatio.Currency,
                CommissionRate = bestRatio.CommissionRate,
                Price = price,
                PayableTotal = payableTotal
            }
        };
    }


}

