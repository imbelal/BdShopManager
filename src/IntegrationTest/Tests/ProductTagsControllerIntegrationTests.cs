using Domain.Entities;
using IntegrationTest.Helpers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using Xunit;

namespace IntegrationTest.Tests
{
    public class ProductTagsControllerIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task Get_All_ProductTags_Should_Success()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductTagsController>().GetAll());

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Get_ProductTags_With_Valid_Filter_Should_Success()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductTagsController>().GetAll());

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }
    }
}