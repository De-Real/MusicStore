using MusicStore.Application.DTOs.Product;

namespace MusicStore.Application.DTOs.Category;

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<ProductResponseDto> Products { get; set; } = [];
}
