using Application.Features.Product.Commands;
using Domain.Entities;
using Domain.Enums;
using IntegrationTest.Helpers;
using Microsoft.EntityFrameworkCore;
using WebApi.Controllers;
using Xunit;

namespace IntegrationTest.Tests
{
    public class ProductIntegrationTests : IntegrationTestBase
    {

        [Fact]
        public async Task Create_Product_Should_Succeed()
        {
            // Arrange
            CreateProductCommand command = new("TestProduct", "TestDesc", Guid.NewGuid(), ProductUnit.Piece, []);

            // Act
            var status = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().Create(command)
            );

            // Assert
            int numberOfProduct = Context.Products.Count();
            Assert.True(numberOfProduct > 0);
        }

        [Fact]
        public async Task Delete_Product_Should_Succeed()
        {
            Product entity = new("Test product", "Test desc", Guid.NewGuid(), ProductUnit.Piece, []);

            // Act
            var status = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().Delete(entity.Id),
                preparedEntities: [entity]
            );

            // Assert
            Product? product = await Context.Products.FirstOrDefaultAsync(x => x.Id == entity.Id, new CancellationToken());
            Assert.Null(product);

        }
    }
}
