using Newtonsoft.Json;

namespace PosSelection.Models.Pos;

public class PosSelectionRequest
{
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("installment")]
    public int Installment { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("card_type")]
    public string? CardType { get; set; }

    [JsonProperty("card_brand")]
    public string? CardBrand { get; set; }
}

