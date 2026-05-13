using MusicStore.Domain.Common;

namespace MusicStore.Application.Common;

public interface IDomainEventDispatcher
{
    Task DispatchAndClearAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken ct = default);
}
