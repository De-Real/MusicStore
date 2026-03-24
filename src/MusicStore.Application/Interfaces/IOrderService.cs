using MusicStore.Application.DTOs.Order;

namespace MusicStore.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderResponseDto>> GetAllAsync();
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<OrderResponseDto>> GetByCustomerAsync(int customerId);
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
}
