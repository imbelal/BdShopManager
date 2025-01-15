using Application.Features.Category.Commands;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Xunit;

namespace IntegrationTest
{
    public class CategoryIntegrationTests : IntegrationTestBase
    {
        public CategoryIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {

        }
        [Fact]
        public async Task Create_Category_ShouldSucceed()
        {
            var entity = new Category("TestCategory");
            var json = System.Text.Json.JsonSerializer.Serialize(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync("api/categories", content);
            httpResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Delete_Category_Should_Delete_Related_products_Also()
        {
            // Arrange
            Category entity = new("TestCate");
            Product product = new("Testproduct", "desc", entity.Id, Domain.Enums.ProductUnit.Piece, new List<Guid>());
            _context.Categories.Add(entity);
            _context.Products.Add(product);
            await _context.SaveChangesAsync(new CancellationToken());

            // Act
            DeleteCategoryCommand command = new(entity.Id);
            await _mediator.Send(command);

            // Assert
            Category? existingCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Id == entity.Id, new CancellationToken());
            List<Product> existingproducts = await _context.Products.Where(x => x.CategoryId == entity.Id).ToListAsync(new CancellationToken());
            Assert.Null(existingCategory);
            Assert.Empty(existingproducts);
        }
    }
}
