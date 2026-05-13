namespace MusicStore.Application.Orders.Commands;

public record CreateOrderCommand(
    int CustomerId,
    List<CreateOrderItemCommand> Items,
    CreateOrderAddressCommand? ShippingAddress = null);

public record CreateOrderItemCommand(int ProductId, int Quantity);

public record CreateOrderAddressCommand(
    string Street,
    string City,
    string Country,
    string? PostalCode = null);
