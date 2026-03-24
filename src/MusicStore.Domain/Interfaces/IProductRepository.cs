using MusicStore.Domain.Entities;

namespace MusicStore.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    Task<Product?> GetWithCategoryAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
}
