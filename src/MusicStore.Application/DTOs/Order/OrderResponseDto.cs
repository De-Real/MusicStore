namespace MusicStore.Application.DTOs.Order;

public class OrderResponseDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerLoyaltyTier { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string[] AllowedNextStatuses { get; set; } = [];
    public decimal OriginalAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalSavings { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = [];
}

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal OriginalUnitPrice { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
    public decimal TierDiscountPercent { get; set; }
    public decimal BulkDiscountPercent { get; set; }
    public decimal PromoDiscountPercent { get; set; }
    public List<string> AppliedDiscounts { get; set; } = [];
}
