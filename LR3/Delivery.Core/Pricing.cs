namespace Delivery.Core;

/* -------------------- STRATEGY -------------------- */
public interface IDiscountStrategy
{
    decimal DiscountAmount(decimal subtotal);
}

public sealed class NoDiscount : IDiscountStrategy
{
    public decimal DiscountAmount(decimal subtotal) => 0m;
}

public sealed class PercentOverThresholdDiscount : IDiscountStrategy
{
    private readonly decimal _threshold;
    private readonly decimal _percent; // 0.10m = 10%

    public PercentOverThresholdDiscount(decimal threshold, decimal percent)
    {
        if (threshold < 0) throw new ArgumentOutOfRangeException(nameof(threshold));
        if (percent < 0 || percent > 1) throw new ArgumentOutOfRangeException(nameof(percent));
        _threshold = threshold;
        _percent = percent;
    }

    public decimal DiscountAmount(decimal subtotal)
        => subtotal >= _threshold ? Math.Round(subtotal * _percent, 2) : 0m;
}

/* -------------------- DECORATOR -------------------- */
public interface ICostCalculator
{
    decimal CalculateTotal(IOrder order);
}

public sealed class SubtotalCalculator : ICostCalculator
{
    public decimal CalculateTotal(IOrder order)
        => order.Items.Sum(x => x.LineTotal);
}

public abstract class CostDecorator : ICostCalculator
{
    protected readonly ICostCalculator Inner;
    protected CostDecorator(ICostCalculator inner) => Inner = inner;
    public abstract decimal CalculateTotal(IOrder order);
}

public sealed class TaxDecorator : CostDecorator
{
    private readonly decimal _taxRate; // 0.21m = 21%
    public TaxDecorator(ICostCalculator inner, decimal taxRate) : base(inner)
    {
        if (taxRate < 0 || taxRate > 1) throw new ArgumentOutOfRangeException(nameof(taxRate));
        _taxRate = taxRate;
    }

    public override decimal CalculateTotal(IOrder order)
    {
        var baseAmount = Inner.CalculateTotal(order);
        var tax = Math.Round(baseAmount * _taxRate, 2);
        return baseAmount + tax;
    }
}

public sealed class DeliveryFeeDecorator : CostDecorator
{
    private readonly decimal _fee;
    public DeliveryFeeDecorator(ICostCalculator inner, decimal fee) : base(inner)
    {
        if (fee < 0) throw new ArgumentOutOfRangeException(nameof(fee));
        _fee = fee;
    }

    public override decimal CalculateTotal(IOrder order) => Inner.CalculateTotal(order) + _fee;
}

public sealed class ExpressFeeDecorator : CostDecorator
{
    private readonly decimal _expressFee;
    public ExpressFeeDecorator(ICostCalculator inner, decimal expressFee) : base(inner)
    {
        if (expressFee < 0) throw new ArgumentOutOfRangeException(nameof(expressFee));
        _expressFee = expressFee;
    }

    public override decimal CalculateTotal(IOrder order)
    {
        var total = Inner.CalculateTotal(order);
        return order.Options?.ExpressDelivery == true ? total + _expressFee : total;
    }
}

public sealed class DiscountDecorator : CostDecorator
{
    private readonly IDiscountStrategy _discount;
    public DiscountDecorator(ICostCalculator inner, IDiscountStrategy discount) : base(inner)
        => _discount = discount;

    public override decimal CalculateTotal(IOrder order)
    {
        // скидку считаем от "подытога" до скидки (обычно subtotal),
        // поэтому берём subtotal отдельно.
        var subtotal = order.Items.Sum(x => x.LineTotal);
        var total = Inner.CalculateTotal(order);
        var discountAmount = _discount.DiscountAmount(subtotal);
        return Math.Max(0m, total - discountAmount);
    }
}