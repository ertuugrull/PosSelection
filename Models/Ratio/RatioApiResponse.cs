using Newtonsoft.Json;

namespace PosSelection.Models.Ration;

public class RatioApiResponse
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

    [JsonProperty("min_fee")]
    public decimal MinFee { get; set; }

    [JsonProperty("priority")]
    public int Priority { get; set; }
}

