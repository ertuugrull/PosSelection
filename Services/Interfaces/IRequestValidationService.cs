using PosSelection.Models.Pos;

namespace PosSelection.Services.Interfaces;

public interface IRequestValidationService
{
    ValidationResult PreCheck(PosSelectionRequest request);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ValidationResult Success() => new() { IsValid = true };

    public static ValidationResult Failure(List<string> errors) => new()
    {
        IsValid = false,
        Errors = errors
    };
}

