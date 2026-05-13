using MusicStore.Domain.Common;
using MusicStore.Domain.Entities;

namespace MusicStore.Domain.Events;

public record OrderCreatedEvent(Order Order, DateTime OccurredAt) : IDomainEvent;
