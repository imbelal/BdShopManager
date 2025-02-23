using Application.Services.Auth.Implementations;
using Application.Services.Common;
using Microsoft.Extensions.Options;

namespace UnitTest
{
    public class PasswordHasherTests
    {
        [Fact]
        public void ShouldCreatePasswordHashAndVerify()
        {
            // Arrange
            var someOptions = Options.Create(new AppSettings { Iterations = 10000 });
            var _passwordHasher = new PasswordHasher(someOptions);
            string password = "Asdf@098765";

            //Act
            var hash = _passwordHasher.CreateHash(password);
            (bool verified, bool needUpgrade) = _passwordHasher.VerifyPassword(hash, password);

            // Assert
            Assert.True(verified);
            Assert.False(needUpgrade);
        }
    }
}
