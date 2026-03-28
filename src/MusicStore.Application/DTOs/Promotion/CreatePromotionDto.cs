using MusicStore.Domain.Entities;

namespace MusicStore.Application.DTOs.Promotion;

public class CreatePromotionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PromotionType Type { get; set; }
    public decimal DiscountPercent { get; set; }
    public bool IsActive { get; set; } = true;

    // For TimeBased
    public DateTime? ActiveFrom { get; set; }
    public DateTime? ActiveTo { get; set; }

    // For CategoryBased
    public int? CategoryId { get; set; }
}
