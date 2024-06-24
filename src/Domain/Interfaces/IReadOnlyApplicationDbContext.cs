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
        IQueryable<Supplier> Suppliers { get; }
        IQueryable<Inventory> Inventories { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<Order> Orders { get; }
    }
}
