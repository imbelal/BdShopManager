using Application.Features.Category.Commands;
using Domain.Entities;
using IntegrationTest.Helpers;
using Microsoft.EntityFrameworkCore;
using WebApi.Controllers;
using Xunit;

namespace IntegrationTest.Tests
{
    public class CategoryIntegrationTests : IntegrationTestBase
    {
        private readonly CategoriesController _categoriesController;

        public CategoryIntegrationTests()
        {
            // Ensure Mediator is not null before passing to the controller
            Assert.NotNull(Mediator); // Add this for safety
            _categoriesController = new CategoriesController(Mediator);
        }

        [Fact]
        public async Task Create_Category_ShouldSucceed()
        {
            // Arrange
            CreateCategoryCommand createCategoryCommand = new("Test");

            // Act
            var status = await ExecuteControllerMethodAsync(
                () => ResolveController<CategoriesController>().Create(createCategoryCommand)
            );

            // Assert
            Category? category = await Context.Categories.FirstOrDefaultAsync(default);
            Assert.NotNull(category);
            Assert.Equal("Test", category.Title);
        }

        [Fact]
        public async Task Delete_Category_Should_Delete_Related_Products_Also()
        {
            // Arrange
            Category entity = new("TestCate");
            Product product = new("TestProduct", "desc", entity.Id, Domain.Enums.ProductUnit.Piece, 0, []);
            DeleteCategoryCommand command = new(entity.Id);

            // Act
            var status = await ExecuteControllerMethodAsync(
                () => ResolveController<CategoriesController>().Delete(command),
                preparedEntities:
                [
                    entity,
                    product
                ]
            );

            // Assert
            Category? existingCategory = await Context.Categories.FirstOrDefaultAsync(x => x.Id == entity.Id, default);
            List<Product> existingProducts = await Context.Products.Where(x => x.CategoryId == entity.Id).ToListAsync(default);
            Assert.Null(existingCategory);
            Assert.Empty(existingProducts);
        }
    }
}