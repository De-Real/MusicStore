using Microsoft.AspNetCore.Mvc;
using MusicStore.Application.DTOs.Customer;
using MusicStore.Application.Interfaces;

namespace MusicStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAll() =>
        Ok(await _customerService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponseDto>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponseDto>> Create(CreateCustomerDto dto)
    {
        try
        {
            var created = await _customerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerResponseDto>> Update(int id, UpdateCustomerDto dto)
    {
        try
        {
            var updated = await _customerService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
