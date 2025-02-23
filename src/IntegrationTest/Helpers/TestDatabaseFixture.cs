using Common.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Testcontainers.MsSql;

namespace IntegrationTest.Helpers
{
    public class TestDatabaseFixture
    {
        private static readonly MsSqlContainer _msSqlContainer;
        private static readonly string _connectionString;
        private static bool _isInitialized;

        static TestDatabaseFixture()
        {
            _msSqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-CU10-ubuntu-22.04")
                .Build();

            _msSqlContainer.StartAsync().GetAwaiter().GetResult();
            _connectionString = _msSqlContainer.GetConnectionString();

            InitializeDatabase();
        }

        public static string ConnectionString => _connectionString;

        private static void InitializeDatabase()
        {
            if (!_isInitialized)
            {
                var options = new DbContextOptionsBuilder<TestDbContext>()
                    .UseSqlServer(_connectionString)
                    .Options;

                using var context = new TestDbContext(
                    new TestDbConnectionProvider(_connectionString),
                    new TestDbContextOptionsProvider(),
                    new Mock<ICurrentUserService>().Object,
                    new Mock<IPublisher>().Object,
                    new Mock<IHostEnvironment>().Object,
                    new Mock<ILogger<TestDbContext>>().Object);

                context.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
                _isInitialized = true;
            }
        }

        public static void Dispose()
        {
            _msSqlContainer?.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
