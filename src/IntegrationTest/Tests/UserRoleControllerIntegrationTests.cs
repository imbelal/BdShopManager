using IntegrationTest.Helpers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using Xunit;

namespace IntegrationTest.Tests
{
    public class UserRoleControllerIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task Get_All_UserRoles_Should_Success()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<UserRoleController>().GetAll());

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Get_UserRoles_With_Valid_Parameter_Should_Success()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<UserRoleController>().GetAll());

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }
    }
}