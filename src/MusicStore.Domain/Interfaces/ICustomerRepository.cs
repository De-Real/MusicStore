using MusicStore.Domain.Entities;

namespace MusicStore.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<bool> EmailExistsAsync(string email);
}
