namespace SaigonRide.Services
{
    public class PricingService
    {
        public bool IsLowInventory(int currentInventory, int capacity)
        {
            if (capacity <= 0) return false;
            return (double)currentInventory / capacity < 0.20;
        }
        public decimal CalculateBaseFare(decimal exactMinutes, decimal pricePerMinute)
        {
            return Math.Round(exactMinutes * pricePerMinute, 0);
        }
        public decimal CalculateFinalFare(decimal baseFare, bool applyDiscount)
        {
            if (!applyDiscount) return baseFare;
            var discount = baseFare * 0.15m;
            return baseFare - discount;
        }
    }
}
