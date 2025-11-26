using PosSelection.Enums;

namespace PosSelection.Helpers
{
    public static class PosHelper
    {
        public static decimal CalculateCost(decimal amount, decimal commissionRate, decimal minFee, string currency)
        {
            decimal baseCost = amount * commissionRate;

            if (currency.Equals(nameof(Currency.USD), StringComparison.OrdinalIgnoreCase))
            {
                baseCost *= 1.01m;
            }

            return Math.Max(baseCost, minFee);
        }
    }
}
