using System;
using System.Linq;
using Inventory.Api.Models;
using Inventory.Api.Services;
using NSubstitute;
using Xunit;

namespace Inventory.UnitTests.Services
{
    public class InventoryServiceTests
    {
        private readonly IInventoryService _sut;
        private readonly IInventoryRepository _repository;

        public InventoryServiceTests()
        {
            _repository = Substitute.For<IInventoryRepository>();

            _repository.TryTake(Arg.Any<Guid>(), Arg.Any<int>(), out Arg.Any<int>())
                .Returns(x =>
                {
                    x[2] = 0;
                    return false;
                });
            
            _sut = new InventoryService(_repository);
        }

        [Fact]
        public void Order_GivenOrderWithNonExistingProduct_ShouldAddItToMissingProducts()
        {
            var productId = Guid.NewGuid();
            var productAmount = 5;
            var orderRow = new OrderRow() { Amount = productAmount, ProductId = productId };

            var result = _sut.Order(new[] {orderRow});
            
            Assert.Equal(1, result.MissingProducts.Count);
            Assert.False(result.Products.Any());
            Assert.Equal(productId, result.MissingProducts.First().ProductId);
            Assert.Equal(productAmount, result.MissingProducts.First().Amount);
        }
        
        [Fact]
        public void Order_GivenOrderWithExistingProduct_ShouldAddItToProducts()
        {
            var productId = Guid.NewGuid();
            var productAmount = 5;
            var orderRow = new OrderRow() { Amount = productAmount, ProductId = productId };
            
            _repository.TryTake(productId, productAmount, out Arg.Any<int>())
                .Returns(x =>
                {
                    x[2] = productAmount;
                    return true;
                });

            var result = _sut.Order(new[] {orderRow});
            
            Assert.False(result.MissingProducts.Any());
            Assert.Equal(1, result.Products.Count);
            Assert.Equal(productId, result.Products.First().ProductId);
            Assert.Equal(productAmount, result.Products.First().Amount);
        }
        
        [Fact]
        public void Order_GivenOrderWithExistingProductAndTooSmallStock_ShouldAddItToBoth()
        {
            var productId = Guid.NewGuid();
            var productAmount = 5;
            var orderRow = new OrderRow() { Amount = productAmount, ProductId = productId };
            
            _repository.TryTake(productId, productAmount, out Arg.Any<int>())
                .Returns(x =>
                {
                    x[2] = 2;
                    return false;
                });

            var result = _sut.Order(new[] {orderRow});
            
            Assert.Equal(1, result.Products.Count);
            Assert.Equal(productId, result.Products.First().ProductId);
            Assert.Equal(2, result.Products.First().Amount);
            
            Assert.Equal(1, result.MissingProducts.Count);
            Assert.Equal(productId, result.MissingProducts.First().ProductId);
            Assert.Equal(3, result.MissingProducts.First().Amount);
        }
    }
}