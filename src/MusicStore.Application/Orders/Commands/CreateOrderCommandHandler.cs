using MusicStore.Application.DTOs.Order;
using MusicStore.Application.Services;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;
using MusicStore.Domain.ValueObjects;

namespace MusicStore.Application.Orders.Commands;

public class CreateOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPromotionRepository _promotionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
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

    public async Task<OrderResponseDto> HandleAsync(CreateOrderCommand command)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var customer = await _customerRepository.GetByIdAsync(command.CustomerId)
                ?? throw new InvalidOperationException($"Customer with id {command.CustomerId} not found.");

            Address? shippingAddress = command.ShippingAddress is { } addr
                ? Address.Create(addr.Street, addr.City, addr.Country, addr.PostalCode)
                : null;

            var order = Order.Create(command.CustomerId, customer, shippingAddress);
            var now = DateTime.UtcNow;

            foreach (var itemCmd in command.Items)
            {
                var product = await _productRepository.GetWithCategoryAsync(itemCmd.ProductId)
                    ?? throw new InvalidOperationException($"Product with id {itemCmd.ProductId} not found.");

                var available = product.StockQuantity - product.ReservedQuantity;
                if (available < itemCmd.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for '{product.Name}'. Available: {available}, requested: {itemCmd.Quantity}.");

                var promotions = await _promotionRepository.GetActiveForProductAsync(product.CategoryId, now);
                var bestPromo = promotions.MaxBy(p => p.DiscountPercent);
                var promoDiscount = bestPromo?.DiscountPercent ?? 0m;
                var promoLabel = bestPromo is not null ? $"Promo '{bestPromo.Name}' ({promoDiscount}% off)" : null;

                var breakdown = PricingCalculator.Calculate(
                    product.Price,
                    itemCmd.Quantity,
                    customer.LoyaltyTier,
                    promoDiscount,
                    promoLabel);

                order.AddItem(
                    product.Id,
                    itemCmd.Quantity,
                    Money.Create(breakdown.OriginalPrice, "UAH"),
                    Money.Create(breakdown.FinalPrice, "UAH"),
                    breakdown.TierDiscountPercent,
                    breakdown.BulkDiscountPercent,
                    breakdown.PromoDiscountPercent);

                product.ReservedQuantity += itemCmd.Quantity;
                _productRepository.Update(product);
            }

            order.Place();

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return OrderMapper.MapToResponse(order);
        });
    }
}
