using Common.ContextBase;
using Common.Services.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class ReadOnlyApplicationDbContext : ApplicationDbContext, IReadOnlyApplicationDbContext
    {
        public ReadOnlyApplicationDbContext(IDbConnectionStringProvider dbConnectionStringProvider, IDbContextOptionsProvider dbContextOptionsProvider, ICurrentUserService _currentUserService, IPublisher _publisher, IHostingEnvironment _hostingEnvironment, ILogger<ApplicationDbContext> logger)
            : base(dbConnectionStringProvider, dbContextOptionsProvider, _currentUserService, _publisher, _hostingEnvironment, logger)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
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
        public new IQueryable<Customer> Customers => base.Customers.AsQueryable();

        public override int SaveChanges()
        {
            throw new InvalidOperationException("This is a read only db context.");
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new InvalidOperationException("This is a read only db context.");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("This is a read only db context.");
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("This is a read only db context.");
        }
    }
}
