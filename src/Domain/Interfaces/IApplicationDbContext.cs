using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Interfaces
{
    public interface IApplicationDbContext : IDisposable
    {
        DbSet<T> GetDbSet<T>() where T : class;
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<ErrorLog> ErrorLogs { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<ProductTag> ProductTags { get; set; }
        DbSet<ProductPhoto> ProductPhotos { get; set; }
        DbSet<Supplier> Suppliers { get; set; }
        DbSet<Inventory> Inventories { get; set; }
        DbSet<Purchase> Purchases { get; set; }
        DbSet<PurchaseItem> PurchaseItems { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<Sales> Sales { get; set; }
        DbSet<SalesItem> SalesItems { get; set; }
        DbSet<Tenant> Tenants { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
