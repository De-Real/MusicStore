using MusicStore.Domain.Entities;

namespace MusicStore.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetWithProductsAsync(int id);
}
