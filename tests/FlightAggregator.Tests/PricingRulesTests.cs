using FlightAggregator.Providers;
using Xunit;

namespace FlightAggregator.Tests;

public sealed class PricingRulesTests
{
    [Theory]
    [InlineData(100.00, 115.00)]
    [InlineData(123.45, 141.97)]
    public void GlobalAir_PerPassengerPrice_AddsFuelSurcharge_AndRounds(decimal baseFare, decimal expected)
    {
        var actual = GlobalAirProvider.CalculatePerPassengerPrice(baseFare);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(100.00, 90.00)]
    [InlineData(40.00, 36.00)]
    [InlineData(20.00, 29.99)]
    public void BudgetWings_PerPassengerPrice_AppliesDiscount_WithMinPrice(decimal baseFare, decimal expected)
    {
        var actual = BudgetWingsProvider.CalculatePerPassengerPrice(baseFare);
        Assert.Equal(expected, actual);
    }
}
