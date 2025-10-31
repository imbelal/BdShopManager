using Common.ContextBase;
using Common.Services.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Infrastructure
{
    public class ApplicationDbContext : EventContextBase<ApplicationDbContext>, IApplicationDbContext, IReadOnlyApplicationDbContext
    {
        public ApplicationDbContext(IDbConnectionStringProvider dbConnectionStringProvider, IDbContextOptionsProvider dbContextOptionsProvider, ICurrentUserService currentUserService, IPublisher publisher, IHostEnvironment hostingEnvironment, ILogger<ApplicationDbContext> logger)
            : base(dbConnectionStringProvider.ConnectionString, dbContextOptionsProvider, currentUserService, publisher, hostingEnvironment, logger)
        {
        }

        public DbSet<T> GetDbSet<T>() where T : class
        {
            return Set<T>();
        }

        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        #region ReadOnly

        IQueryable<UserRole> IReadOnlyApplicationDbContext.UserRoles => UserRoles.AsQueryable();
        IQueryable<ErrorLog> IReadOnlyApplicationDbContext.ErrorLogs => ErrorLogs.AsQueryable();
        IQueryable<User> IReadOnlyApplicationDbContext.Users => Users.AsQueryable();
        IQueryable<RefreshToken> IReadOnlyApplicationDbContext.RefreshTokens => RefreshTokens.AsQueryable();
        IQueryable<Category> IReadOnlyApplicationDbContext.Categories => Categories.AsQueryable();
        IQueryable<Tag> IReadOnlyApplicationDbContext.Tags => Tags.AsQueryable();
        IQueryable<Product> IReadOnlyApplicationDbContext.Products => Products.AsQueryable();
        IQueryable<ProductTag> IReadOnlyApplicationDbContext.ProductTags => ProductTags.AsQueryable();
        IQueryable<ProductPhoto> IReadOnlyApplicationDbContext.ProductPhotos => ProductPhotos.AsQueryable();
        IQueryable<Supplier> IReadOnlyApplicationDbContext.Suppliers => Suppliers.AsQueryable();
        IQueryable<Inventory> IReadOnlyApplicationDbContext.Inventories => Inventories.AsQueryable();
        IQueryable<Purchase> IReadOnlyApplicationDbContext.Purchases => Purchases.AsQueryable();
        IQueryable<PurchaseItem> IReadOnlyApplicationDbContext.PurchaseItems => PurchaseItems.AsQueryable();
        IQueryable<Customer> IReadOnlyApplicationDbContext.Customers => Customers.AsQueryable();
        IQueryable<Order> IReadOnlyApplicationDbContext.Orders => Orders.AsQueryable();
        IQueryable<OrderDetail> IReadOnlyApplicationDbContext.OrderDetails => OrderDetails.AsQueryable();
        IQueryable<Payment> IReadOnlyApplicationDbContext.Payments => Payments.AsQueryable();
        IQueryable<Tenant> IReadOnlyApplicationDbContext.Tenants => Tenants.AsQueryable();
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add all external entity configurations
            Assembly assemblyWithConfigurations = typeof(TenantConfiguration).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Find the 'Id' property
                var idProperty = entityType.FindProperty("Id");

                if (idProperty != null && entityType.ClrType != typeof(ErrorLog))
                {
                    idProperty.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;
                }
            }
        }
    }
}
