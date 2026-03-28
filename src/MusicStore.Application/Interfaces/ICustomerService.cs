using MusicStore.Application.DTOs.Customer;

namespace MusicStore.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDto>> GetAllAsync();
    Task<CustomerResponseDto?> GetByIdAsync(int id);
    Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerResponseDto?> UpdateAsync(int id, UpdateCustomerDto dto);
}
