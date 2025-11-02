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
        public async Task Delete_Category_With_Products_Should_Throw_Exception()
        {
            // Arrange
            Category entity = new("TestCate");
            Product product = new("TestProduct", "desc", "Medium", "Red", entity.Id, Domain.Enums.ProductUnit.Piece, 0, []);
            DeleteCategoryCommand command = new(entity.Id);

            // Act & Assert
            await Assert.ThrowsAsync<Common.Exceptions.BusinessLogicException>(async () =>
            {
                await ExecuteControllerMethodAsync(
                    () => ResolveController<CategoriesController>().Delete(command),
                    preparedEntities:
                    [
                        entity,
                        product
                    ]
                );
            });

            // Verify category and product still exist
            Category? existingCategory = await Context.Categories.FirstOrDefaultAsync(x => x.Id == entity.Id, default);
            Product? existingProduct = await Context.Products.FirstOrDefaultAsync(x => x.CategoryId == entity.Id, default);
            Assert.NotNull(existingCategory);
            Assert.NotNull(existingProduct);
        }

        [Fact]
        public async Task Delete_Category_Without_Products_Should_Succeed()
        {
            // Arrange
            Category entity = new("TestCate");
            DeleteCategoryCommand command = new(entity.Id);

            // Act
            var status = await ExecuteControllerMethodAsync(
                () => ResolveController<CategoriesController>().Delete(command),
                preparedEntities: [entity]
            );

            // Assert
            Category? existingCategory = await Context.Categories.FirstOrDefaultAsync(x => x.Id == entity.Id, default);
            Assert.Null(existingCategory);
        }
    }
}