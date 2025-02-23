using Common.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Testcontainers.MsSql;
using Xunit;

namespace IntegrationTest.Helpers
{
    public class TestDatabaseFixture : IDisposable
    {
        private static readonly MsSqlContainer _msSqlContainer;
        private static readonly string _connectionString;
        private static bool _isInitialized;

        static TestDatabaseFixture()
        {
            // Check if running on Azure Pipeline agent
            var isRunningInPipeline = Environment.GetEnvironmentVariable("TF_BUILD") == "True";

            if (isRunningInPipeline)
            {
                // Use TestContainers in Azure Pipeline
                _msSqlContainer = new MsSqlBuilder()
                    .WithImage("mcr.microsoft.com/mssql/server:2022-CU10-ubuntu-22.04")
                    .Build();

                _msSqlContainer.StartAsync().GetAwaiter().GetResult();
                _connectionString = _msSqlContainer.GetConnectionString();
            }
            else
            {
                // Use mssqllocaldb locally
                _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=TestDb;Trusted_Connection=True;MultipleActiveResultSets=true";
            }

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

        public void Dispose()
        {
            if (_msSqlContainer != null)
            {
                _msSqlContainer?.DisposeAsync().GetAwaiter().GetResult();
            }
            else
            {
                using var context = new TestDbContext(
                    new TestDbConnectionProvider(_connectionString),
                    new TestDbContextOptionsProvider(),
                    new Mock<ICurrentUserService>().Object,
                    new Mock<IPublisher>().Object,
                    new Mock<IHostEnvironment>().Object,
                    new Mock<ILogger<TestDbContext>>().Object);

                // Drop all Foreign Key Constraints
                context.Database.ExecuteSqlRaw(@"
                DECLARE @sql NVARCHAR(MAX) = '';
                    SELECT @sql += 'ALTER TABLE [' + sch.name + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + '];'
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
                    INNER JOIN sys.schemas sch ON t.schema_id = sch.schema_id;
                    EXEC sp_executesql @sql;
                ");

                // Drop all Tables
                context.Database.ExecuteSqlRaw(@"
                    EXEC sp_MSforeachtable 'DROP TABLE ?';
                ");

                // Drop all Views
                context.Database.ExecuteSqlRaw(@"
                    DECLARE @sql NVARCHAR(MAX) = '';
                    SELECT @sql += 'DROP VIEW [' + s.name + '].[' + v.name + '];'
                    FROM sys.views v
                    INNER JOIN sys.schemas s ON v.schema_id = s.schema_id;
                    EXEC sp_executesql @sql;
                ");

                // Drop all Functions
                context.Database.ExecuteSqlRaw(@"
                    DECLARE @sql NVARCHAR(MAX) = '';
                    SELECT @sql += 'DROP FUNCTION [' + s.name + '].[' + f.name + '];'
                    FROM sys.objects f
                    INNER JOIN sys.schemas s ON f.schema_id = s.schema_id
                    WHERE f.type IN ('FN', 'IF', 'TF'); -- Scalar, Inline, Table-valued functions
                    EXEC sp_executesql @sql;
                ");

                // Drop all Stored Procedures
                context.Database.ExecuteSqlRaw(@"
                    DECLARE @sql NVARCHAR(MAX) = '';
                    SELECT @sql += 'DROP PROCEDURE [' + s.name + '].[' + p.name + '];'
                    FROM sys.procedures p
                    INNER JOIN sys.schemas s ON p.schema_id = s.schema_id;
                    EXEC sp_executesql @sql;
                ");
            }
        }
    }

    [CollectionDefinition("DatabaseCollection")]
    public class DatabaseCollection : ICollectionFixture<TestDatabaseFixture>
    {
    }
}
