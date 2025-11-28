using PosSelection.Enums;

namespace PosSelection.Helpers
{
    public static class PosHelper
    {
        public static decimal CalculateCost(decimal amount, decimal commissionRate, decimal minFee, string currency)
        {
            decimal baseCost = amount * commissionRate;

            decimal currencyMultiplier = currency switch
            {
                nameof(Currency.USD) => 1.01m,
                nameof(Currency.TRY) => 1.00m,
                _ => 1.00m
            };

            baseCost *= currencyMultiplier;

            return Math.Max(baseCost, minFee);
        }
    }
}
