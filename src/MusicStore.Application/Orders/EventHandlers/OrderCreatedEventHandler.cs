using Microsoft.Extensions.Logging;
using MusicStore.Application.Common;
using MusicStore.Domain.Events;

namespace MusicStore.Application.Orders.EventHandlers;

public class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[Domain Event] Order #{OrderId} created for Customer #{CustomerId}. Total: {Total}",
            domainEvent.Order.Id,
            domainEvent.Order.CustomerId,
            domainEvent.Order.TotalPrice);

        return Task.CompletedTask;
    }
}
