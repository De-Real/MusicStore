using MusicStore.Application.DTOs.Promotion;
using MusicStore.Application.Interfaces;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;

namespace MusicStore.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;

    public PromotionService(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<IEnumerable<PromotionResponseDto>> GetAllAsync()
    {
        var promotions = await _promotionRepository.GetAllAsync();
        return promotions.Select(MapToResponse);
    }

    public async Task<PromotionResponseDto?> GetByIdAsync(int id)
    {
        var promotion = await _promotionRepository.GetByIdAsync(id);
        return promotion is null ? null : MapToResponse(promotion);
    }

    public async Task<PromotionResponseDto> CreateAsync(CreatePromotionDto dto)
    {
        var promotion = new Promotion
        {
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type,
            DiscountPercent = dto.DiscountPercent,
            IsActive = dto.IsActive,
            ActiveFrom = dto.ActiveFrom,
            ActiveTo = dto.ActiveTo,
            CategoryId = dto.CategoryId
        };
        await _promotionRepository.AddAsync(promotion);
        await _promotionRepository.SaveChangesAsync();
        return MapToResponse(promotion);
    }

    public async Task<PromotionResponseDto?> UpdateAsync(int id, CreatePromotionDto dto)
    {
        var promotion = await _promotionRepository.GetByIdAsync(id);
        if (promotion is null) return null;

        promotion.Name = dto.Name;
        promotion.Description = dto.Description;
        promotion.Type = dto.Type;
        promotion.DiscountPercent = dto.DiscountPercent;
        promotion.IsActive = dto.IsActive;
        promotion.ActiveFrom = dto.ActiveFrom;
        promotion.ActiveTo = dto.ActiveTo;
        promotion.CategoryId = dto.CategoryId;

        _promotionRepository.Update(promotion);
        await _promotionRepository.SaveChangesAsync();
        return MapToResponse(promotion);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var promotion = await _promotionRepository.GetByIdAsync(id);
        if (promotion is null) return false;

        _promotionRepository.Delete(promotion);
        await _promotionRepository.SaveChangesAsync();
        return true;
    }

    private static PromotionResponseDto MapToResponse(Promotion p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Type = p.Type.ToString(),
        DiscountPercent = p.DiscountPercent,
        IsActive = p.IsActive,
        ActiveFrom = p.ActiveFrom,
        ActiveTo = p.ActiveTo,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name
    };
}
