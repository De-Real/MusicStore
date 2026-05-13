using MusicStore.Domain.Common;
using MusicStore.Domain.Events;
using MusicStore.Domain.ValueObjects;

namespace MusicStore.Domain.Entities;

public class Order : AggregateRoot
{
    public int Id { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalPrice { get; private set; } = null!;
    public Address? ShippingAddress { get; private set; }

    public int CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { }

    public static Order Create(int customerId, Customer customer, Address? shippingAddress = null)
    {
        return new Order
        {
            CustomerId = customerId,
            Customer = customer,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalPrice = Money.Zero(),
            ShippingAddress = shippingAddress
        };
    }

    public void AddItem(
        int productId,
        int quantity,
        Money originalUnitPrice,
        Money unitPrice,
        decimal tierDiscountPercent,
        decimal bulkDiscountPercent,
        decimal promoDiscountPercent)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Items can only be added to a pending order.");

        _orderItems.Add(new OrderItem(
            productId, quantity,
            originalUnitPrice, unitPrice,
            tierDiscountPercent, bulkDiscountPercent, promoDiscountPercent));

        RecalculateTotal();
    }

    public void Place()
    {
        if (_orderItems.Count == 0)
            throw new InvalidOperationException("Cannot place an order with no items.");

        RaiseDomainEvent(new OrderCreatedEvent(this, DateTime.UtcNow));
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        var allowed = GetAllowedTransitions();
        if (!allowed.Contains(newStatus))
        {
            var allowedStr = allowed.Length > 0
                ? string.Join(", ", allowed.Select(s => s.ToString()))
                : "none";
            throw new InvalidOperationException(
                $"Cannot transition from '{Status}' to '{newStatus}'. Allowed: {allowedStr}.");
        }

        Status = newStatus;
    }

    public OrderStatus[] GetAllowedTransitions() => Status switch
    {
        OrderStatus.Pending   => [OrderStatus.Confirmed, OrderStatus.Cancelled],
        OrderStatus.Confirmed => [OrderStatus.Shipped,   OrderStatus.Cancelled],
        OrderStatus.Shipped   => [OrderStatus.Delivered],
        _                     => []
    };

    private void RecalculateTotal()
    {
        var total = _orderItems.Sum(i => i.UnitPrice * i.Quantity);
        TotalPrice = Money.Create(Math.Round(total, 2), "UAH");
    }
}
