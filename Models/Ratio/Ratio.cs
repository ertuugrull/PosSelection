namespace PosSelection.Models.Ration;

public class Ratio
{
    public string PosName { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string CardBrand { get; set; } = string.Empty;
    public int Installment { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal CommissionRate { get; set; }
    public decimal MinFee { get; set; }
    public int Priority { get; set; }
}

