using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;
using MusicStore.Infrastructure.Data;

namespace MusicStore.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync() =>
        await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

    public async Task<Product?> GetWithCategoryAsync(int id) =>
        await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId) =>
        await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
}
