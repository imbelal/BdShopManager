using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IReadOnlyApplicationDbContext
    {
        IQueryable<UserRole> UserRoles { get; }
        IQueryable<ErrorLog> ErrorLogs { get; }
        IQueryable<User> Users { get; }
        IQueryable<RefreshToken> RefreshTokens { get; }
        IQueryable<Category> Categories { get; }
        IQueryable<Tag> Tags { get; }
        IQueryable<Product> Products { get; }
        IQueryable<ProductTag> ProductTags { get; }
        IQueryable<ProductPhoto> ProductPhotos { get; }
        IQueryable<Supplier> Suppliers { get; }
        IQueryable<Purchase> Purchases { get; }
        IQueryable<PurchaseItem> PurchaseItems { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<Sales> Sales { get; }
        IQueryable<SalesItem> SalesItems { get; }
        IQueryable<SalesReturn> SalesReturns { get; }
        IQueryable<SalesReturnItem> SalesReturnItems { get; }
        IQueryable<Payment> Payments { get; }
        IQueryable<StockTransaction> StockTransactions { get; }
        IQueryable<StockAdjustment> StockAdjustments { get; }
        IQueryable<Tenant> Tenants { get; }
    }
}
