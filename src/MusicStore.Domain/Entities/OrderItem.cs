namespace MusicStore.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal OriginalUnitPrice { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TierDiscountPercent { get; set; }
    public decimal BulkDiscountPercent { get; set; }
    public decimal PromoDiscountPercent { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
