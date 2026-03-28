using MusicStore.Application.DTOs.Promotion;

namespace MusicStore.Application.Interfaces;

public interface IPromotionService
{
    Task<IEnumerable<PromotionResponseDto>> GetAllAsync();
    Task<PromotionResponseDto?> GetByIdAsync(int id);
    Task<PromotionResponseDto> CreateAsync(CreatePromotionDto dto);
    Task<PromotionResponseDto?> UpdateAsync(int id, CreatePromotionDto dto);
    Task<bool> DeleteAsync(int id);
}
