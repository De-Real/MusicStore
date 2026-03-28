using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;
using MusicStore.Infrastructure.Data;

namespace MusicStore.Infrastructure.Repositories;

public class PromotionRepository : Repository<Promotion>, IPromotionRepository
{
    public PromotionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Promotion>> GetActiveForProductAsync(int categoryId, DateTime at)
    {
        return await _context.Promotions
            .Where(p => p.IsActive && (
                (p.Type == PromotionType.TimeBased && p.ActiveFrom <= at && p.ActiveTo >= at) ||
                (p.Type == PromotionType.CategoryBased && p.CategoryId == categoryId)
            ))
            .ToListAsync();
    }
}
