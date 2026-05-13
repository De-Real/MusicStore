using MusicStore.Domain.ValueObjects;

namespace MusicStore.Domain.Entities;

public class OrderItem
{
    public int Id { get; private set; }
    public int Quantity { get; private set; }
    public decimal OriginalUnitPrice { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TierDiscountPercent { get; private set; }
    public decimal BulkDiscountPercent { get; private set; }
    public decimal PromoDiscountPercent { get; private set; }

    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    private OrderItem() { }

    internal OrderItem(
        int productId,
        int quantity,
        Money originalUnitPrice,
        Money unitPrice,
        decimal tierDiscountPercent,
        decimal bulkDiscountPercent,
        decimal promoDiscountPercent)
    {
        ProductId = productId;
        Quantity = quantity;
        OriginalUnitPrice = originalUnitPrice.Amount;
        UnitPrice = unitPrice.Amount;
        TierDiscountPercent = tierDiscountPercent;
        BulkDiscountPercent = bulkDiscountPercent;
        PromoDiscountPercent = promoDiscountPercent;
    }
}
