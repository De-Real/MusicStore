using MusicStore.Application.DTOs.Product;
using MusicStore.Application.Interfaces;
using MusicStore.Domain.Entities;
using MusicStore.Domain.Interfaces;

namespace MusicStore.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllWithCategoryAsync();
        return products.Select(MapToResponse);
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetWithCategoryAsync(id);
        return product is null ? null : MapToResponse(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Brand = dto.Brand,
            CategoryId = dto.CategoryId
        };
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
        return MapToResponse(product);
    }

    public async Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null) return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.Brand = dto.Brand;
        product.CategoryId = dto.CategoryId;

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();
        return MapToResponse(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null) return false;

        _productRepository.Delete(product);
        await _productRepository.SaveChangesAsync();
        return true;
    }

    private static ProductResponseDto MapToResponse(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        ReservedQuantity = p.ReservedQuantity,
        AvailableQuantity = p.StockQuantity - p.ReservedQuantity,
        Brand = p.Brand,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name
    };
}
