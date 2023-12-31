using Common.ContextBase;
using Common.Services.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntegrationTest
{
    public class TestDbContext : ApplicationDbContext, IReadOnlyApplicationDbContext
    {
        public TestDbContext(IDbConnectionStringProvider dbConnectionStringProvider, IDbContextOptionsProvider dbContextOptionsProvider, ICurrentUserService currentUserService, IPublisher publisher, IHostingEnvironment hostingEnvironment, ILogger<TestDbContext> logger)
        : base(dbConnectionStringProvider, dbContextOptionsProvider, currentUserService, publisher, hostingEnvironment, logger)
        {
            if (!Database.IsSqlServer())
            {
                var dbState = Database.GetDbConnection().State;
                if (dbState == System.Data.ConnectionState.Closed)
                {
                    Database.OpenConnection();
                }
                Database.EnsureCreated();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SQLiteModelBuilderService.OnModelCreating(modelBuilder);
        }

        public new IQueryable<UserRole> UserRoles => base.UserRoles.AsQueryable();
        public new IQueryable<ErrorLog> ErrorLogs => base.ErrorLogs.AsQueryable();
        public new IQueryable<User> Users => base.Users.AsQueryable();
        public new IQueryable<RefreshToken> RefreshTokens => base.RefreshTokens.AsQueryable();
        public new IQueryable<Category> Categories => base.Categories.AsQueryable();
        public new IQueryable<Tag> Tags => base.Tags.AsQueryable();
        public new IQueryable<Product> Products => base.Products.AsQueryable();
        public new IQueryable<ProductTag> ProductTags => base.ProductTags.AsQueryable();
        public new IQueryable<Supplier> Suppliers => base.Suppliers.AsQueryable();
        public new IQueryable<Inventory> Inventories => base.Inventories.AsQueryable();
    }
}
