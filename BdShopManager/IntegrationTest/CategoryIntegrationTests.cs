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
        public async Task Delete_Category_Should_Delete_Related_Posts_Also()
        {
            // Arrange
            Category entity = new("TestCate")
            {
                Id = Guid.NewGuid()
            };
            Product post = new("TestPost", "desc", entity.Id, 5, Domain.Enums.ProductUnit.Piece, new List<Guid>())
            {
                Id = Guid.NewGuid()
            };
            _context.Categories.Add(entity);
            _context.Products.Add(post);
            await _context.SaveChangesAsync(new CancellationToken());

            // Act
            DeleteCategoryCommand command = new(entity.Id);
            await _mediator.Send(command);

            // Assert
            Category? existingCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Id == entity.Id, new CancellationToken());
            List<Product> existingPosts = await _context.Products.Where(x => x.CategoryId == entity.Id).ToListAsync(new CancellationToken());
            Assert.Null(existingCategory);
            Assert.Empty(existingPosts);
        }
    }
}
