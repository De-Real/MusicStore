using MusicStore.Domain.Entities;

namespace MusicStore.Domain.Interfaces;

public interface IPromotionRepository : IRepository<Promotion>
{
    Task<IEnumerable<Promotion>> GetActiveForProductAsync(int categoryId, DateTime at);
}
