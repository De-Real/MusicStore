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
    private readonly IPromotionRepository _promotionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IPromotionRepository promotionRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _promotionRepository = promotionRepository;
        _unitOfWork = unitOfWork;
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
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId)
                ?? throw new InvalidOperationException($"Customer with id {dto.CustomerId} not found.");

            var now = DateTime.UtcNow;
            var orderItems = new List<OrderItem>();
            decimal discountedTotal = 0;

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository.GetWithCategoryAsync(itemDto.ProductId)
                    ?? throw new InvalidOperationException($"Product with id {itemDto.ProductId} not found.");

                var available = product.StockQuantity - product.ReservedQuantity;
                if (available < itemDto.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for '{product.Name}'. Available: {available}, requested: {itemDto.Quantity}.");

                var promotions = await _promotionRepository.GetActiveForProductAsync(product.CategoryId, now);
                var bestPromo = promotions.MaxBy(p => p.DiscountPercent);
                var promoDiscount = bestPromo?.DiscountPercent ?? 0m;
                var promoLabel = bestPromo is not null ? $"Promo '{bestPromo.Name}' ({promoDiscount}% off)" : null;

                var breakdown = PricingCalculator.Calculate(
                    product.Price,
                    itemDto.Quantity,
                    customer.LoyaltyTier,
                    promoDiscount,
                    promoLabel);

                product.ReservedQuantity += itemDto.Quantity;
                _productRepository.Update(product);

                discountedTotal += breakdown.FinalPrice * itemDto.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = itemDto.Quantity,
                    OriginalUnitPrice = breakdown.OriginalPrice,
                    UnitPrice = breakdown.FinalPrice,
                    TierDiscountPercent = breakdown.TierDiscountPercent,
                    BulkDiscountPercent = breakdown.BulkDiscountPercent,
                    PromoDiscountPercent = breakdown.PromoDiscountPercent
                });
            }

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                Customer = customer,
                OrderDate = now,
                Status = OrderStatus.Pending,
                TotalAmount = Math.Round(discountedTotal, 2),
                OrderItems = orderItems
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
            return MapToResponse(order);
        });
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

            if (!OrderStatusWorkflow.CanTransition(order.Status, newStatus))
            {
                var allowed = OrderStatusWorkflow.GetAllowedTransitions(order.Status);
                var allowedStr = allowed.Length > 0 ? string.Join(", ", allowed) : "none";
                throw new InvalidOperationException(
                    $"Cannot transition from '{order.Status}' to '{newStatus}'. Allowed: {allowedStr}.");
            }

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

                customer.TotalSpent += order.TotalAmount;
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

            order.Status = newStatus;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
            return MapToResponse(order);
        });
    }

    private static OrderResponseDto MapToResponse(Order o)
    {
        var originalAmount = o.OrderItems.Sum(i => i.OriginalUnitPrice * i.Quantity);
        return new OrderResponseDto
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer is not null
                ? $"{o.Customer.FirstName} {o.Customer.LastName}"
                : string.Empty,
            CustomerLoyaltyTier = o.Customer?.LoyaltyTier.ToString() ?? string.Empty,
            OrderDate = o.OrderDate,
            Status = o.Status.ToString(),
            AllowedNextStatuses = OrderStatusWorkflow.GetAllowedTransitions(o.Status),
            OriginalAmount = originalAmount,
            TotalAmount = o.TotalAmount,
            TotalSavings = Math.Round(originalAmount - o.TotalAmount, 2),
            Items = o.OrderItems.Select(i =>
            {
                var discounts = new List<string>();
                if (i.TierDiscountPercent > 0)
                    discounts.Add($"{o.Customer?.LoyaltyTier} loyalty tier ({i.TierDiscountPercent}% off)");
                if (i.BulkDiscountPercent > 0)
                    discounts.Add($"Bulk discount ({i.BulkDiscountPercent}% off)");
                if (i.PromoDiscountPercent > 0)
                    discounts.Add($"Promotion ({i.PromoDiscountPercent}% off)");

                return new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    OriginalUnitPrice = i.OriginalUnitPrice,
                    UnitPrice = i.UnitPrice,
                    TierDiscountPercent = i.TierDiscountPercent,
                    BulkDiscountPercent = i.BulkDiscountPercent,
                    PromoDiscountPercent = i.PromoDiscountPercent,
                    AppliedDiscounts = discounts
                };
            }).ToList()
        };
    }
}
