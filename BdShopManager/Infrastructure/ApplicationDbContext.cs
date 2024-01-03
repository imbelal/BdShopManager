using Common.ContextBase;
using Common.Services.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Infrastructure
{
    public class ApplicationDbContext : EventContextBase<ApplicationDbContext>, IApplicationDbContext
    {
        public ApplicationDbContext(IDbConnectionStringProvider dbConnectionStringProvider, IDbContextOptionsProvider dbContextOptionsProvider, ICurrentUserService currentUserService, IPublisher publisher, IHostingEnvironment hostingEnvironment, ILogger<ApplicationDbContext> logger)
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
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add all external entity configurations
            Assembly assemblyWithConfigurations = typeof(ErrorLogConfiguration).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);
            base.OnModelCreating(modelBuilder);
        }
    }
}
