using Newtonsoft.Json;

namespace PosSelection.Models.Pos;

public class PosSelectionResponse
{
    [JsonProperty("filters")]
    public PosSelectionFilters Filters { get; set; } = new();
    
    [JsonProperty("overall_min")]
    public PosSelectionResult OverallMin { get; set; } = new();
}

public class PosSelectionFilters
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

public class PosSelectionResult
{
    [JsonProperty("pos_name")]
    public string PosName { get; set; } = string.Empty;
    
    [JsonProperty("card_type")]
    public string CardType { get; set; } = string.Empty;
    
    [JsonProperty("card_brand")]
    public string CardBrand { get; set; } = string.Empty;
    
    [JsonProperty("installment")]
    public int Installment { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;
    
    [JsonProperty("commission_rate")]
    public decimal CommissionRate { get; set; }
    
    [JsonProperty("price")]
    public decimal Price { get; set; }
    
    [JsonProperty("payable_total")]
    public decimal PayableTotal { get; set; }
}

