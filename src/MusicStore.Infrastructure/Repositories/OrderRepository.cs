using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;
using MusicStore.Infrastructure.Data;

namespace MusicStore.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync() =>
        await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .ToListAsync();

    public async Task<Order?> GetWithDetailsAsync(int id) =>
        await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId) =>
        await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .Where(o => o.CustomerId == customerId)
            .ToListAsync();
}
