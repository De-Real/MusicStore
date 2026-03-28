namespace MusicStore.Domain.Entities;

public class Promotion
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PromotionType Type { get; set; }
    public decimal DiscountPercent { get; set; }
    public bool IsActive { get; set; }

    // For TimeBased
    public DateTime? ActiveFrom { get; set; }
    public DateTime? ActiveTo { get; set; }

    // For CategoryBased
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}
