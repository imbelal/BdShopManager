using Application.Features.Category.Commands;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Controllers;
using Xunit;

namespace IntegrationTest
{
    public class CategoryIntegrationTests : IntegrationTestBase
    {
        private CategoriesController _categoriesController;

        public CategoryIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
            _categoriesController = new CategoriesController(_mediator);
        }

        [Fact]
        public async Task Create_Category_ShouldSucceed()
        {
            CreateCategoryCommand createCategoryCommand = new CreateCategoryCommand("Test");

            var status = await _categoriesController.Create(createCategoryCommand);

            Category category = await _context.Categories.FirstAsync(default);
            Assert.NotNull(category);
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
