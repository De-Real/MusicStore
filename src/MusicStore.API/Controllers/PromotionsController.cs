using Microsoft.AspNetCore.Mvc;
using MusicStore.Application.DTOs.Promotion;
using MusicStore.Application.Interfaces;

namespace MusicStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PromotionResponseDto>>> GetAll() =>
        Ok(await _promotionService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PromotionResponseDto>> GetById(int id)
    {
        var promotion = await _promotionService.GetByIdAsync(id);
        return promotion is null ? NotFound() : Ok(promotion);
    }

    [HttpPost]
    public async Task<ActionResult<PromotionResponseDto>> Create(CreatePromotionDto dto)
    {
        var created = await _promotionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PromotionResponseDto>> Update(int id, CreatePromotionDto dto)
    {
        var updated = await _promotionService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _promotionService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
