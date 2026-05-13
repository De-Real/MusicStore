using MusicStore.Application.DTOs.Order;
using MusicStore.Application.Interfaces;
using MusicStore.Application.Orders;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;

namespace MusicStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllWithDetailsAsync();
        return orders.Select(OrderMapper.MapToResponse);
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        return order is null ? null : OrderMapper.MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetByCustomerAsync(int customerId)
    {
        var orders = await _orderRepository.GetByCustomerAsync(customerId);
        return orders.Select(OrderMapper.MapToResponse);
    }

    public async Task<OrderResponseDto> UpdateStatusAsync(int id, UpdateOrderStatusDto dto)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await _orderRepository.GetWithDetailsAsync(id)
                ?? throw new InvalidOperationException($"Order with id {id} not found.");

            if (!Enum.TryParse<OrderStatus>(dto.Status, ignoreCase: true, out var newStatus))
                throw new InvalidOperationException(
                    $"Invalid status '{dto.Status}'. Valid values: {string.Join(", ", Enum.GetNames<OrderStatus>())}");

            order.ChangeStatus(newStatus);

            if (newStatus == OrderStatus.Delivered)
            {
                var customer = await _customerRepository.GetByIdAsync(order.CustomerId)
                    ?? throw new InvalidOperationException($"Customer with id {order.CustomerId} not found.");

                foreach (var item in order.OrderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId)
                        ?? throw new InvalidOperationException($"Product with id {item.ProductId} not found.");

                    product.StockQuantity -= item.Quantity;
                    product.ReservedQuantity -= item.Quantity;
                    _productRepository.Update(product);
                }

                customer.TotalSpent += order.TotalPrice.Amount;
                _customerRepository.Update(customer);
            }
            else if (newStatus == OrderStatus.Cancelled)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId)
                        ?? throw new InvalidOperationException($"Product with id {item.ProductId} not found.");

                    product.ReservedQuantity -= item.Quantity;
                    _productRepository.Update(product);
                }
            }

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
            return OrderMapper.MapToResponse(order);
        });
    }
}
