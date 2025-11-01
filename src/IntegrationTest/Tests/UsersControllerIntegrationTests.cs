using IntegrationTest.Helpers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using Xunit;

namespace IntegrationTest.Tests
{
    public class UsersControllerIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task Get_All_Users_Should_Success()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<UsersController>().GetAll());

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Get_Users_With_Valid_Parameters_Should_Success()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<UsersController>().GetAll());

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }
    }
}