using System.Collections.ObjectModel;

namespace Delivery.Core;

/*
ПАТТЕРНЫ (>=5) и зачем:
1) Factory (OrderFactory) — создание разных типов заказа (Standard/Special) без размазывания new по коду.
2) Builder (OrderBuilder) — удобное пошаговое построение заказа (позиции, опции), расширяемо без ломки конструкторов.
3) State (Preparing/Delivering/Completed) — управление статусами и переходами без if/else по всему проекту.
4) Strategy (IDiscountStrategy) — смена/добавление правил скидок без изменения калькулятора.
5) Decorator (CostCalculator decorators) — гибкая сборка расчёта стоимости (налоги/доставка/экспресс/скидка) как цепочка.
6) Observer (IOrderObserver) — подписчики на изменения статуса заказа (лог/уведомления) без жёсткой связности.
*/

public enum OrderKind { Standard, Special }

public sealed record MenuItem(string Id, string Name, decimal Price);

public sealed record OrderItem(MenuItem Item, int Quantity)
{
    public decimal LineTotal => Item.Price * Quantity;
}

public sealed record SpecialOptions(bool ExpressDelivery, string? PreferencesNote);

public interface IOrder
{
    Guid Id { get; }
    string CustomerName { get; }
    string Address { get; }
    IReadOnlyList<OrderItem> Items { get; }
    string Status { get; }
    SpecialOptions? Options { get; }

    void AddItem(MenuItem item, int quantity);
    void AdvanceStatus(); // подготовка -> доставка -> выполнен
    void Subscribe(IOrderObserver observer);
    void Unsubscribe(IOrderObserver observer);
}

public interface IOrderObserver
{
    void OnStatusChanged(Guid orderId, string newStatus);
}

/* -------------------- STATE -------------------- */
public interface IOrderState
{
    string Name { get; }
    IOrderState Next();
}

public sealed class PreparingState : IOrderState
{
    public string Name => "Подготовка";
    public IOrderState Next() => new DeliveringState();
}

public sealed class DeliveringState : IOrderState
{
    public string Name => "Доставка";
    public IOrderState Next() => new CompletedState();
}

public sealed class CompletedState : IOrderState
{
    public string Name => "Выполнен";
    public IOrderState Next() => this; // дальше не продвигаем
}

/* -------------------- ORDERS -------------------- */
public abstract class OrderBase : IOrder
{
    private readonly List<OrderItem> _items = new();
    private readonly List<IOrderObserver> _observers = new();
    private IOrderState _state = new PreparingState();

    protected OrderBase(string customerName, string address)
    {
        Id = Guid.NewGuid();
        CustomerName = customerName;
        Address = address;
    }

    public Guid Id { get; }
    public string CustomerName { get; }
    public string Address { get; }
    public IReadOnlyList<OrderItem> Items => new ReadOnlyCollection<OrderItem>(_items);
    public string Status => _state.Name;

    public abstract SpecialOptions? Options { get; }

    public void AddItem(MenuItem item, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        _items.Add(new OrderItem(item, quantity));
    }

    public void AdvanceStatus()
    {
        var next = _state.Next();
        if (!ReferenceEquals(_state, next))
        {
            _state = next;
            Notify();
        }
    }

    public void Subscribe(IOrderObserver observer)
    {
        if (!_observers.Contains(observer)) _observers.Add(observer);
    }

    public void Unsubscribe(IOrderObserver observer) => _observers.Remove(observer);

    private void Notify()
    {
        foreach (var o in _observers.ToArray())
            o.OnStatusChanged(Id, Status);
    }
}

public sealed class StandardOrder : OrderBase
{
    public StandardOrder(string customerName, string address) : base(customerName, address) { }
    public override SpecialOptions? Options => null;
}

public sealed class SpecialOrder : OrderBase
{
    private readonly SpecialOptions _options;
    public SpecialOrder(string customerName, string address, SpecialOptions options) : base(customerName, address)
        => _options = options;

    public override SpecialOptions? Options => _options;
}

/* -------------------- FACTORY -------------------- */
public interface IOrderFactory
{
    IOrder Create(OrderKind kind, string customerName, string address, SpecialOptions? options = null);
}

public sealed class OrderFactory : IOrderFactory
{
    public IOrder Create(OrderKind kind, string customerName, string address, SpecialOptions? options = null)
        => kind switch
        {
            OrderKind.Standard => new StandardOrder(customerName, address),
            OrderKind.Special => new SpecialOrder(customerName, address, options ?? new SpecialOptions(false, null)),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
}

/* -------------------- BUILDER -------------------- */
public sealed class OrderBuilder
{
    private readonly IOrderFactory _factory;
    private readonly List<(MenuItem item, int qty)> _lines = new();

    private OrderKind _kind = OrderKind.Standard;
    private string _customer = "Unknown";
    private string _address = "Unknown";
    private SpecialOptions? _options;

    public OrderBuilder(IOrderFactory factory) => _factory = factory;

    public OrderBuilder Standard(string customer, string address)
    {
        _kind = OrderKind.Standard;
        _customer = customer;
        _address = address;
        _options = null;
        return this;
    }

    public OrderBuilder Special(string customer, string address, bool expressDelivery, string? preferencesNote = null)
    {
        _kind = OrderKind.Special;
        _customer = customer;
        _address = address;
        _options = new SpecialOptions(expressDelivery, preferencesNote);
        return this;
    }

    public OrderBuilder Add(MenuItem item, int qty)
    {
        _lines.Add((item, qty));
        return this;
    }

    public IOrder Build()
    {
        var order = _factory.Create(_kind, _customer, _address, _options);
        foreach (var (item, qty) in _lines) order.AddItem(item, qty);
        return order;
    }
}