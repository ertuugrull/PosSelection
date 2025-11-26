namespace PosSelection.Extensions
{
    public static class DecimalExtension
    {
        public static decimal RoundHalfUp(this decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
