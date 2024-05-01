using Moq;
using Sharebook.Core.Interfaces;
using Sharebook.Core.Models;
using Sharebook.Services;

namespace Sharebook.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task CreateProduct_ValidProduct_ReturnsTrue()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var productService = new ProductService(unitOfWorkMock.Object);
            var product = new Product
            {
                Name = "Product A",
                Description = "Version 123",
                Price = 1000,
                ProductStock = 1
            };

            unitOfWorkMock.Setup(u => u.Products.Add(It.IsAny<Product>()));
            unitOfWorkMock.Setup(u => u.Save()).Returns(1);

            // Act
            var result = await productService.CreateProduct(product);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProduct_ValidProductId_ReturnsTrue()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var productService = new ProductService(unitOfWorkMock.Object);
            int productId = 1;

            var productToDelete = new Product { Id = productId };

            unitOfWorkMock.Setup(u => u.Products.GetById(productId)).ReturnsAsync(productToDelete);
            unitOfWorkMock.Setup(u => u.Products.Delete(It.IsAny<Product>()));
            unitOfWorkMock.Setup(u => u.Save()).Returns(1);

            // Act
            var result = await productService.DeleteProduct(productId);

            // Assert
            Assert.True(result);
        }
    }
}
