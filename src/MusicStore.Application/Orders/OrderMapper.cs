using MusicStore.Application.DTOs.Order;
using MusicStore.Domain.Entities;

namespace MusicStore.Application.Orders;

internal static class OrderMapper
{
    internal static OrderResponseDto MapToResponse(Order o)
    {
        var originalAmount = o.OrderItems.Sum(i => i.OriginalUnitPrice * i.Quantity);
        return new OrderResponseDto
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer is not null
                ? $"{o.Customer.FirstName} {o.Customer.LastName}"
                : string.Empty,
            CustomerLoyaltyTier = o.Customer?.LoyaltyTier.ToString() ?? string.Empty,
            OrderDate = o.OrderDate,
            Status = o.Status.ToString(),
            AllowedNextStatuses = o.GetAllowedTransitions().Select(s => s.ToString()).ToArray(),
            OriginalAmount = originalAmount,
            TotalAmount = o.TotalPrice.Amount,
            TotalSavings = Math.Round(originalAmount - o.TotalPrice.Amount, 2),
            Items = o.OrderItems.Select(i =>
            {
                var discounts = new List<string>();
                if (i.TierDiscountPercent > 0)
                    discounts.Add($"{o.Customer?.LoyaltyTier} loyalty tier ({i.TierDiscountPercent}% off)");
                if (i.BulkDiscountPercent > 0)
                    discounts.Add($"Bulk discount ({i.BulkDiscountPercent}% off)");
                if (i.PromoDiscountPercent > 0)
                    discounts.Add($"Promotion ({i.PromoDiscountPercent}% off)");

                return new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    OriginalUnitPrice = i.OriginalUnitPrice,
                    UnitPrice = i.UnitPrice,
                    TierDiscountPercent = i.TierDiscountPercent,
                    BulkDiscountPercent = i.BulkDiscountPercent,
                    PromoDiscountPercent = i.PromoDiscountPercent,
                    AppliedDiscounts = discounts
                };
            }).ToList()
        };
    }
}
