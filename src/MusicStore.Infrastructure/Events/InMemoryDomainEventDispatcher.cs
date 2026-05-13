using Microsoft.Extensions.DependencyInjection;
using MusicStore.Application.Common;
using MusicStore.Domain.Common;

namespace MusicStore.Infrastructure.Events;

public class InMemoryDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryDomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAndClearAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken ct = default)
    {
        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    if (handler is not null)
                        await ((dynamic)handler).HandleAsync((dynamic)domainEvent, ct);
                }
            }

            aggregate.ClearDomainEvents();
        }
    }
}
