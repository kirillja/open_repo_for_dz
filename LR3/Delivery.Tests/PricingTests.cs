using Delivery.Core;
using Xunit;

namespace Delivery.Tests;

public sealed class PricingTests
{
    [Fact]
    public void CostCalculator_Works_With_Decorators_And_DiscountStrategy()
    {
        // subtotal = 2*10 + 1*5 = 25
        // delivery fee = 3
        // express fee = 4 (special express)
        // tax 20% на (subtotal + delivery + express) = (25+3+4)*0.2 = 6.40
        // discount 10% если subtotal >= 20 => 2.50
        // total = 25 + 3 + 4 + 6.40 - 2.50 = 35.90

        var a = new MenuItem("a", "Burger", 10m);
        var b = new MenuItem("b", "Fries", 5m);

        var order = new OrderBuilder(new OrderFactory())
            .Special("Ivan", "Riga", expressDelivery: true)
            .Add(a, 2)
            .Add(b, 1)
            .Build();

        ICostCalculator calc =
            new DiscountDecorator(
                new TaxDecorator(
                    new ExpressFeeDecorator(
                        new DeliveryFeeDecorator(
                            new SubtotalCalculator(),
                            fee: 3m),
                        expressFee: 4m),
                    taxRate: 0.20m),
                discount: new PercentOverThresholdDiscount(threshold: 20m, percent: 0.10m)
            );

        var total = calc.CalculateTotal(order);

        Assert.Equal(35.90m, total);
    }
}