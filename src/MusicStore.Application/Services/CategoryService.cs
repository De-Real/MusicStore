using MusicStore.Application.DTOs.Category;
using MusicStore.Application.DTOs.Product;
using MusicStore.Application.Interfaces;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;

namespace MusicStore.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetWithProductsAsync(id);
        if (category is null) return null;

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Products = category.Products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Brand = p.Brand,
                CategoryId = p.CategoryId,
                CategoryName = category.Name
            }).ToList()
        };
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return new CategoryResponseDto { Id = category.Id, Name = category.Name, Description = category.Description };
    }

    public async Task<CategoryResponseDto?> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null) return null;

        category.Name = dto.Name;
        category.Description = dto.Description;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync();
        return new CategoryResponseDto { Id = category.Id, Name = category.Name, Description = category.Description };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null) return false;

        _categoryRepository.Delete(category);
        await _categoryRepository.SaveChangesAsync();
        return true;
    }
}
