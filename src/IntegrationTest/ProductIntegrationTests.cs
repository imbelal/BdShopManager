using Application.Features.Product.Commands;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTest
{
    public class ProductIntegrationTests : IntegrationTestBase
    {

        public ProductIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {

        }

        [Fact]
        public async Task Create_Product_Should_Succeed()
        {
            // Arrange
            CreateProductCommand command = new("Testproduct", "TestDesc", Guid.NewGuid(), ProductUnit.Piece, new List<Guid>());

            // Act
            await _mediator.Send(command);

            // Assert
            int numberOfproduct = _context.Products.Count();
            Assert.True(numberOfproduct > 0);
        }

        [Fact]
        public async Task Delete_Product_Should_Succeed()
        {
            Product entity = new("Test product", "Test desc", Guid.NewGuid(), ProductUnit.Piece, new List<Guid>());

            _context.Products.Add(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            // Act
            DeleteProductCommand deleteproductCommand = new(entity.Id);
            await _mediator.Send(deleteproductCommand);

            // Assert
            Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == entity.Id, new CancellationToken());
            Assert.Null(product);

        }
    }
}
