using MusicStore.Domain.Entities;

namespace MusicStore.Application.Services;

public static class OrderStatusWorkflow
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        [OrderStatus.Pending]   = [OrderStatus.Confirmed, OrderStatus.Cancelled],
        [OrderStatus.Confirmed] = [OrderStatus.Shipped, OrderStatus.Cancelled],
        [OrderStatus.Shipped]   = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = [],
        [OrderStatus.Cancelled] = [],
    };

    public static bool CanTransition(OrderStatus from, OrderStatus to) =>
        AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    public static string[] GetAllowedTransitions(OrderStatus from) =>
        AllowedTransitions.TryGetValue(from, out var allowed)
            ? allowed.Select(s => s.ToString()).ToArray()
            : [];
}
