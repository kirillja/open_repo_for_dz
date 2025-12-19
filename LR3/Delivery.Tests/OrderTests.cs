using Delivery.Core;
using Xunit;

namespace Delivery.Tests;

public sealed class OrderTests
{
    [Fact]
    public void Factory_Creates_Standard_And_Special()
    {
        IOrderFactory factory = new OrderFactory();

        var a = factory.Create(OrderKind.Standard, "Ivan", "Riga");
        var b = factory.Create(OrderKind.Special, "Petr", "Riga", new SpecialOptions(true, "no onion"));

        Assert.IsType<StandardOrder>(a);
        Assert.IsType<SpecialOrder>(b);
        Assert.Null(a.Options);
        Assert.True(b.Options!.ExpressDelivery);
    }

    [Fact]
    public void Builder_Builds_Order_With_Items()
    {
        var pizza = new MenuItem("p1", "Pizza", 10m);
        var cola  = new MenuItem("c1", "Cola", 2m);

        var order = new OrderBuilder(new OrderFactory())
            .Special("Anna", "Riga", expressDelivery: true, preferencesNote: "extra cheese")
            .Add(pizza, 2)
            .Add(cola, 3)
            .Build();

        Assert.Equal(2, order.Items.Count);
        Assert.Equal("Подготовка", order.Status);
        Assert.True(order.Options!.ExpressDelivery);
    }

    [Fact]
    public void State_Transitions_Preparing_To_Delivering_To_Completed()
    {
        var order = new OrderFactory().Create(OrderKind.Standard, "Ivan", "Riga");

        Assert.Equal("Подготовка", order.Status);

        order.AdvanceStatus();
        Assert.Equal("Доставка", order.Status);

        order.AdvanceStatus();
        Assert.Equal("Выполнен", order.Status);

        order.AdvanceStatus(); // дальше не меняется
        Assert.Equal("Выполнен", order.Status);
    }

    private sealed class TestObserver : IOrderObserver
    {
        public int Calls { get; private set; }
        public string? LastStatus { get; private set; }
        public void OnStatusChanged(Guid orderId, string newStatus)
        {
            Calls++;
            LastStatus = newStatus;
        }
    }

    [Fact]
    public void Observer_Is_Notified_On_Status_Change()
    {
        var order = new OrderFactory().Create(OrderKind.Standard, "Ivan", "Riga");
        var obs = new TestObserver();
        order.Subscribe(obs);

        order.AdvanceStatus();

        Assert.Equal(1, obs.Calls);
        Assert.Equal("Доставка", obs.LastStatus);
    }
}