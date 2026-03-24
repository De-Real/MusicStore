using MusicStore.Application.DTOs.Customer;
using MusicStore.Application.Interfaces;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;

namespace MusicStore.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToResponse);
    }

    public async Task<CustomerResponseDto?> GetByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer is null ? null : MapToResponse(customer);
    }

    public async Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto)
    {
        if (await _customerRepository.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException($"A customer with email '{dto.Email}' already exists.");

        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address
        };
        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();
        return MapToResponse(customer);
    }

    private static CustomerResponseDto MapToResponse(Customer c) => new()
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName,
        Email = c.Email,
        Phone = c.Phone,
        Address = c.Address
    };
}
