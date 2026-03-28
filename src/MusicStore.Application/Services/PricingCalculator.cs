using MusicStore.Domain.Entities;

namespace MusicStore.Application.Services;

public record PriceBreakdown(
    decimal OriginalPrice,
    decimal FinalPrice,
    decimal TierDiscountPercent,
    decimal BulkDiscountPercent,
    decimal PromoDiscountPercent,
    string? TierDiscountLabel,
    string? BulkDiscountLabel,
    string? PromoDiscountLabel
);

public static class PricingCalculator
{
    public static decimal GetTierDiscount(LoyaltyTier tier) => tier switch
    {
        LoyaltyTier.Silver => 5m,
        LoyaltyTier.Gold => 10m,
        _ => 0m
    };

    public static decimal GetBulkDiscount(int quantity) => quantity switch
    {
        >= 5 => 10m,
        >= 3 => 5m,
        _ => 0m
    };

    public static PriceBreakdown Calculate(
        decimal basePrice,
        int quantity,
        LoyaltyTier loyaltyTier,
        decimal promoDiscountPercent,
        string? promoLabel)
    {
        var tierDiscount = GetTierDiscount(loyaltyTier);
        var bulkDiscount = GetBulkDiscount(quantity);

        var price = basePrice;
        price -= price * (tierDiscount / 100m);
        price -= price * (bulkDiscount / 100m);
        price -= price * (promoDiscountPercent / 100m);
        price = Math.Round(price, 2);

        return new PriceBreakdown(
            OriginalPrice: basePrice,
            FinalPrice: price,
            TierDiscountPercent: tierDiscount,
            BulkDiscountPercent: bulkDiscount,
            PromoDiscountPercent: promoDiscountPercent,
            TierDiscountLabel: tierDiscount > 0 ? $"{loyaltyTier} loyalty tier ({tierDiscount}% off)" : null,
            BulkDiscountLabel: bulkDiscount > 0 ? $"Bulk discount qty≥{(quantity >= 5 ? 5 : 3)} ({bulkDiscount}% off)" : null,
            PromoDiscountLabel: promoDiscountPercent > 0 ? promoLabel : null
        );
    }
}
