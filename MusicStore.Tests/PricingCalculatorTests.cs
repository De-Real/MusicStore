using MusicStore.Application.Services;
using MusicStore.Domain.Entities;

namespace MusicStore.Tests;

public class PricingCalculatorTests
{
    // --- GetTierDiscount ---

    [Fact]
    public void GetTierDiscount_Bronze_ReturnsZero()
    {
        Assert.Equal(0m, PricingCalculator.GetTierDiscount(LoyaltyTier.Bronze));
    }

    [Fact]
    public void GetTierDiscount_Silver_ReturnsFivePercent()
    {
        Assert.Equal(5m, PricingCalculator.GetTierDiscount(LoyaltyTier.Silver));
    }

    [Fact]
    public void GetTierDiscount_Gold_ReturnsTenPercent()
    {
        Assert.Equal(10m, PricingCalculator.GetTierDiscount(LoyaltyTier.Gold));
    }

    // --- GetBulkDiscount ---

    [Fact]
    public void GetBulkDiscount_Qty1_ReturnsZero()
    {
        Assert.Equal(0m, PricingCalculator.GetBulkDiscount(1));
    }

    [Fact]
    public void GetBulkDiscount_Qty2_ReturnsZero()
    {
        Assert.Equal(0m, PricingCalculator.GetBulkDiscount(2));
    }

    [Fact]
    public void GetBulkDiscount_Qty3_ReturnsFivePercent()
    {
        Assert.Equal(5m, PricingCalculator.GetBulkDiscount(3));
    }

    [Fact]
    public void GetBulkDiscount_Qty4_ReturnsFivePercent()
    {
        Assert.Equal(5m, PricingCalculator.GetBulkDiscount(4));
    }

    [Fact]
    public void GetBulkDiscount_Qty5_ReturnsTenPercent()
    {
        Assert.Equal(10m, PricingCalculator.GetBulkDiscount(5));
    }

    [Fact]
    public void GetBulkDiscount_Qty10_ReturnsTenPercent()
    {
        Assert.Equal(10m, PricingCalculator.GetBulkDiscount(10));
    }

    // --- Calculate: no discounts ---

    [Fact]
    public void Calculate_BronzeTier_Qty1_NoPromo_ReturnsSamePrice()
    {
        var result = PricingCalculator.Calculate(1000m, 1, LoyaltyTier.Bronze, 0m, null);
        Assert.Equal(1000m, result.FinalPrice);
        Assert.Equal(0m, result.TierDiscountPercent);
        Assert.Equal(0m, result.BulkDiscountPercent);
        Assert.Equal(0m, result.PromoDiscountPercent);
    }

    // --- Calculate: tier discount only ---

    [Fact]
    public void Calculate_SilverTier_Qty1_NoPromo_AppliesFivePercentDiscount()
    {
        var result = PricingCalculator.Calculate(1000m, 1, LoyaltyTier.Silver, 0m, null);
        Assert.Equal(950m, result.FinalPrice);
        Assert.Equal(5m, result.TierDiscountPercent);
        Assert.NotNull(result.TierDiscountLabel);
    }

    [Fact]
    public void Calculate_GoldTier_Qty1_NoPromo_AppliesTenPercentDiscount()
    {
        var result = PricingCalculator.Calculate(1000m, 1, LoyaltyTier.Gold, 0m, null);
        Assert.Equal(900m, result.FinalPrice);
        Assert.Equal(10m, result.TierDiscountPercent);
    }

    // --- Calculate: bulk discount only ---

    [Fact]
    public void Calculate_BronzeTier_Qty3_NoPromo_AppliesFivePercentBulk()
    {
        var result = PricingCalculator.Calculate(1000m, 3, LoyaltyTier.Bronze, 0m, null);
        Assert.Equal(950m, result.FinalPrice);
        Assert.Equal(5m, result.BulkDiscountPercent);
    }

    [Fact]
    public void Calculate_BronzeTier_Qty5_NoPromo_AppliesTenPercentBulk()
    {
        var result = PricingCalculator.Calculate(1000m, 5, LoyaltyTier.Bronze, 0m, null);
        Assert.Equal(900m, result.FinalPrice);
        Assert.Equal(10m, result.BulkDiscountPercent);
    }

    // --- Calculate: stacked discounts ---

    [Fact]
    public void Calculate_GoldTier_Qty5_NoPromo_StacksTierAndBulk()
    {
        // Gold: -10% → 900, then Bulk -10% → 810
        var result = PricingCalculator.Calculate(1000m, 5, LoyaltyTier.Gold, 0m, null);
        Assert.Equal(810m, result.FinalPrice);
        Assert.Equal(10m, result.TierDiscountPercent);
        Assert.Equal(10m, result.BulkDiscountPercent);
    }

    [Fact]
    public void Calculate_GoldTier_Qty5_WithPromo_StacksAllThree()
    {
        // Gold: -10% → 900, Bulk -10% → 810, Promo -20% → 648
        var result = PricingCalculator.Calculate(1000m, 5, LoyaltyTier.Gold, 20m, "Summer Sale");
        Assert.Equal(648m, result.FinalPrice);
        Assert.Equal(10m, result.TierDiscountPercent);
        Assert.Equal(10m, result.BulkDiscountPercent);
        Assert.Equal(20m, result.PromoDiscountPercent);
        Assert.NotNull(result.PromoDiscountLabel);
    }

    // --- Calculate: zero promo has no label ---

    [Fact]
    public void Calculate_ZeroPromoDiscount_PromoLabelIsNull()
    {
        var result = PricingCalculator.Calculate(500m, 1, LoyaltyTier.Bronze, 0m, "Some promo");
        Assert.Null(result.PromoDiscountLabel);
    }

    // --- OriginalPrice is always preserved ---

    [Fact]
    public void Calculate_OriginalPriceIsUnchanged()
    {
        var result = PricingCalculator.Calculate(1234.56m, 5, LoyaltyTier.Gold, 15m, "Promo");
        Assert.Equal(1234.56m, result.OriginalPrice);
    }
}
