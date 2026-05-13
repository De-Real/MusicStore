using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MusicStore.Application.Common;
using MusicStore.Domain.Common;

namespace MusicStore.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MusicStoreDb;Username=andriivynarchuk;Password=");
        return new AppDbContext(optionsBuilder.Options, new NoOpDomainEventDispatcher());
    }

    private sealed class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public Task DispatchAndClearAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
