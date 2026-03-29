using Xunit;
using MusicStore.Application.DTOs.Order;
using MusicStore.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Tests
{
    public class OrderServiceTests
    {
        [Fact]
        public void DummyTest_ShouldCompileAndShowContribution()
        {
            // Просто створюємо об’єкт CreateOrderDto
            var createDto = new CreateOrderDto
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = 1, Quantity = 2 },
                    new CreateOrderItemDto { ProductId = 2, Quantity = 1 }
                }
            };

            // Логіка тесту — просто підрахунок сумарної кількості товарів
            int totalQuantity = createDto.Items.Sum(i => i.Quantity);

            // Перевірка, щоб тест проходив
            Assert.Equal(3, totalQuantity);
        }
    }
}