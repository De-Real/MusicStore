using Microsoft.AspNetCore.Mvc;
using MusicStore.Application.DTOs.Order;
using MusicStore.Application.Interfaces;
using MusicStore.Application.Orders.Commands;

namespace MusicStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly CreateOrderCommandHandler _createOrderHandler;

    public OrdersController(IOrderService orderService, CreateOrderCommandHandler createOrderHandler)
    {
        _orderService = orderService;
        _createOrderHandler = createOrderHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll() =>
        Ok(await _orderService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("customer/{customerId:int}")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetByCustomer(int customerId) =>
        Ok(await _orderService.GetByCustomerAsync(customerId));

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create(CreateOrderDto dto)
    {
        try
        {
            var command = new CreateOrderCommand(
                dto.CustomerId,
                dto.Items.Select(i => new CreateOrderItemCommand(i.ProductId, i.Quantity)).ToList(),
                dto.AddressStreet is not null
                    ? new CreateOrderAddressCommand(
                        dto.AddressStreet,
                        dto.AddressCity ?? string.Empty,
                        dto.AddressCountry ?? string.Empty,
                        dto.AddressPostalCode)
                    : null);

            var created = await _createOrderHandler.HandleAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<OrderResponseDto>> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        try
        {
            var updated = await _orderService.UpdateStatusAsync(id, dto);
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
