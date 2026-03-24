using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;
using MusicStore.Infrastructure.Data;

namespace MusicStore.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<Category?> GetWithProductsAsync(int id) =>
        await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
}
