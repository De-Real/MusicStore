using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;
using MusicStore.Infrastructure.Data;

namespace MusicStore.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public async Task<bool> EmailExistsAsync(string email) =>
        await _context.Customers.AnyAsync(c => c.Email == email);
}
