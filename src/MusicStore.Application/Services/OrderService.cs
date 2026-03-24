using MusicStore.Application.DTOs.Order;
using MusicStore.Application.Interfaces;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;

namespace MusicStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllWithDetailsAsync();
        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetWithDetailsAsync(id);
        return order is null ? null : MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetByCustomerAsync(int customerId)
    {
        var orders = await _orderRepository.GetByCustomerAsync(customerId);
        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(dto.CustomerId)
            ?? throw new InvalidOperationException($"Customer with id {dto.CustomerId} not found.");

        var orderItems = new List<OrderItem>();
        decimal total = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId)
                ?? throw new InvalidOperationException($"Product with id {itemDto.ProductId} not found.");

            if (product.StockQuantity < itemDto.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}, requested: {itemDto.Quantity}.");

            product.StockQuantity -= itemDto.Quantity;
            _productRepository.Update(product);

            total += product.Price * itemDto.Quantity;
            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });
        }

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            Customer = customer,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = total,
            OrderItems = orderItems
        };

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();
        return MapToResponse(order);
    }

    private static OrderResponseDto MapToResponse(Order o) => new()
    {
        Id = o.Id,
        CustomerId = o.CustomerId,
        CustomerName = o.Customer is not null
            ? $"{o.Customer.FirstName} {o.Customer.LastName}"
            : string.Empty,
        OrderDate = o.OrderDate,
        Status = o.Status.ToString(),
        TotalAmount = o.TotalAmount,
        Items = o.OrderItems.Select(i => new OrderItemResponseDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product?.Name ?? string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList()
    };
}
