using PosSelection.Enums;
using PosSelection.Models.Pos;
using PosSelection.Services.Interfaces;

namespace PosSelection.Services;

public class RequestValidationService(ILogger<RequestValidationService> logger) : IRequestValidationService
{
    private readonly ILogger<RequestValidationService> _logger = logger;

    // fluent validationda kullan�labilirdi ama business logic devreye girebilir , servis katman�nda yapmamda sak�nca yok
    public ValidationResult PreCheck(PosSelectionRequest request)
    {
        var errors = new List<string>();

        if (request.Amount <= 0)
        {
            errors.Add("Amount must be greater than 0");
        }

        if (request.Installment <= 0 || request.Installment > 12)
        {
            errors.Add("Installment must be between 1 and 12");
        }

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            errors.Add("Currency is required and cannot be empty");
        }
        else if (!Enum.TryParse<Currency>(request.Currency, ignoreCase: true, out _))
        {
            var validCurrencies = string.Join(", ", Enum.GetNames<Currency>());
            errors.Add($"Currency must be one of: {validCurrencies}");
        }

        if (!string.IsNullOrWhiteSpace(request.CardType))
        {
            if (!Enum.TryParse<CardType>(request.CardType, ignoreCase: true, out _))
            {
                var validCardTypes = string.Join(", ", Enum.GetNames<CardType>());
                errors.Add($"CardType must be one of: {validCardTypes} or empty");
            }
        }

        if (errors.Count != 0)
        {
            _logger.LogWarning("Request validation failed. Errors: {Errors}", string.Join(", ", errors));
            return ValidationResult.Failure(errors);
        }

        _logger.LogDebug("Request validation passed");
        return ValidationResult.Success();
    }
}

