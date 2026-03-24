using MusicStore.Domain.Entities;

namespace MusicStore.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetAllWithDetailsAsync();
    Task<Order?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);
}
