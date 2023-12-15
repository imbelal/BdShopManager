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
            CreateProductCommand command = new("TestPost", "TestDesc", Guid.NewGuid(), 5, ProductUnit.Piece, new List<Guid>());

            // Act
            await _mediator.Send(command);

            // Assert
            int numberOfPost = _context.Products.Count();
            Assert.True(numberOfPost > 0);
        }

        [Fact]
        public async Task Delete_Product_Should_Succeed()
        {
            // Arrange
            Product entity = new("Test post", "Test desc", Guid.NewGuid(), 5, ProductUnit.Piece, new List<Guid>())
            {
                Id = Guid.NewGuid()
            };
            _context.Products.Add(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            // Act
            DeleteProductCommand deletePostCommand = new(entity.Id);
            await _mediator.Send(deletePostCommand);

            // Assert
            Product? post = await _context.Products.FirstOrDefaultAsync(x => x.Id == entity.Id, new CancellationToken());
            Assert.Null(post);

        }
    }
}
