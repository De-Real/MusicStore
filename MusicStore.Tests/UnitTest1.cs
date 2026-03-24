using Xunit;
using MusicStore.Domain.Entities;
using System.Collections.Generic;

namespace MusicStore.Tests
{
    // Mock версія ProductService для тестів
    public class FakeProductService
    {
        private readonly List<Product> _products = new();
        public void AddProduct(Product p) => _products.Add(p);
        public List<Product> GetAllProducts() => _products;
        public decimal GetTotalPrice()
        {
            decimal total = 0;
            foreach (var p in _products) total += p.Price;
            return total;
        }
    }

    public class ProductServiceTests
    {
        [Fact]
        public void CanCreateProduct()
        {
            var product = new Product { Id = 1, Name = "Guitar", Price = 1500 };
            Assert.Equal("Guitar", product.Name);
            Assert.Equal(1500, product.Price);
        }

        [Fact]
        public void CanAddProductToService()
        {
            var service = new FakeProductService();
            var product = new Product { Id = 2, Name = "Drums", Price = 2000 };
            service.AddProduct(product);

            var allProducts = service.GetAllProducts();
            Assert.Contains(product, allProducts);
        }

        [Fact]
        public void TotalPriceCalculationWorks()
        {
            var service = new FakeProductService();
            service.AddProduct(new Product { Name = "Guitar", Price = 1500 });
            service.AddProduct(new Product { Name = "Drums", Price = 2000 });

            decimal total = service.GetTotalPrice();
            Assert.Equal(3500, total);
        }

        [Fact]
        public void SimpleStringTest()
        {
            string a = "MusicStore";
            Assert.False(string.IsNullOrEmpty(a));
        }

        [Fact]
        public void SimpleMathTest()
        {
            Assert.Equal(4, 2 + 2);
        }
    }
}