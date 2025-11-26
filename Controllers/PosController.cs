using Microsoft.AspNetCore.Mvc;
using PosSelection.Models.Pos;
using PosSelection.Services.Interfaces;

namespace PosSelection.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PosController(
    IPosSelectionService posSelectionService,
    IRequestValidationService validationService,
    ILogger<PosController> logger) : ControllerBase
{
    private readonly IPosSelectionService _posSelectionService = posSelectionService;
    private readonly IRequestValidationService _validationService = validationService;
    private readonly ILogger<PosController> _logger = logger;

    [HttpPost("v1/selectBest")]
    public async Task<ActionResult<PosSelectionResponse>> SelectBest([FromBody] PosSelectionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = _validationService.PreCheck(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { error = "Validation failed", errors = validationResult.Errors });
            }

            var response = await _posSelectionService.SelectBestPosAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "POS seçimi başarısız: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POS seçimi sırasında beklenmeyen hata");
            return StatusCode(500, new { error = "İç sunucu hatası" });
        }
    }
}

