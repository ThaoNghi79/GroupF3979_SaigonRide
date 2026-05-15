using Xunit;
using SaigonRide.Services;

namespace SaigonRide.Tests
{
    public class PricingServiceTests
    {
        private readonly PricingService _svc = new PricingService();

        [Fact]
        public void EP01_Utilization10Percent_ShouldApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 1, capacity: 10);
            Assert.True(result);
        }

        [Fact]
        public void EP02_Utilization15Percent_ShouldApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 3, capacity: 20);
            Assert.True(result);
        }

        [Fact]
        public void EP03_Utilization20Percent_ShouldNotApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 2, capacity: 10);
            Assert.False(result);
        }

        [Fact]
        public void EP04_Utilization50Percent_ShouldNotApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 5, capacity: 10);
            Assert.False(result);
        }

        [Fact]
        public void EP05_Utilization100Percent_ShouldNotApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 10, capacity: 10);
            Assert.False(result);
        }

        [Fact]
        public void BVA01_Utilization0Percent_MinBoundary_ShouldApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 0, capacity: 10);
            Assert.True(result);
        }

        [Fact]
        public void BVA02_Utilization10Percent_InsideRange_ShouldApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 1, capacity: 10);
            Assert.True(result);
        }

        [Fact]
        public void BVA03_Utilization19Percent_JustBelow_ShouldApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 19, capacity: 100);
            Assert.True(result);
        }

        [Fact]
        public void BVA04_Utilization20Percent_ExactBoundary_ShouldNotApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 2, capacity: 10);
            Assert.False(result);
        }

        [Fact]
        public void BVA05_Utilization20Percent_AnotherCase_ShouldNotApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 20, capacity: 100);
            Assert.False(result);
        }

        [Fact]
        public void BVA06_Utilization30Percent_AboveBoundary_ShouldNotApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 3, capacity: 10);
            Assert.False(result);
        }

        [Fact]
        public void BVA07_Utilization100Percent_MaxBoundary_ShouldNotApplyDiscount()
        {
            var result = _svc.IsLowInventory(currentInventory: 10, capacity: 10);
            Assert.False(result);
        }


        [Fact]
        public void Fare_StandardBike_10Min_NoDiscount_ShouldBe5000()
        {
            var baseFare = _svc.CalculateBaseFare(exactMinutes: 10m, pricePerMinute: 500m);
            var finalFare = _svc.CalculateFinalFare(baseFare, applyDiscount: false);
            Assert.Equal(5000m, finalFare);
        }

        [Fact]
        public void Fare_EScooter_10Min_NoDiscount_ShouldBe15000()
        {
            var baseFare = _svc.CalculateBaseFare(exactMinutes: 10m, pricePerMinute: 1500m);
            var finalFare = _svc.CalculateFinalFare(baseFare, applyDiscount: false);
            Assert.Equal(15000m, finalFare);
        }

        [Fact]
        public void Fare_StandardBike_10Min_WithDiscount_ShouldBe4250()
        {
            var baseFare = _svc.CalculateBaseFare(exactMinutes: 10m, pricePerMinute: 500m);
            var finalFare = _svc.CalculateFinalFare(baseFare, applyDiscount: true);
            Assert.Equal(4250m, finalFare);
        }

        [Fact]
        public void Fare_EScooter_10Min_WithDiscount_ShouldBe12750()
        {
            var baseFare = _svc.CalculateBaseFare(exactMinutes: 10m, pricePerMinute: 1500m);
            var finalFare = _svc.CalculateFinalFare(baseFare, applyDiscount: true);
            Assert.Equal(12750m, finalFare);
        }
    }
}
