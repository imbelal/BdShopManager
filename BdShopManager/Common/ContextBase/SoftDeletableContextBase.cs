using Common.Interceptor;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Common.ContextBase
{
    public class SoftDeletableContextBase<TContext> : DbContext where TContext : DbContext
    {
        public SoftDeletableContextBase(DbContextOptions options) :
            base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureSoftDeleteFilter(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureSoftDeleteFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Check if the entity type has an IsDeleted property
                var isDeletedProperty = entityType.FindProperty("IsDeleted");
                if (isDeletedProperty != null && isDeletedProperty.ClrType == typeof(bool))
                {
                    // Configure the filter to exclude soft-deleted entities
                    var parameter = Expression.Parameter(entityType.ClrType);
                    var filter = Expression.Lambda(
                        Expression.NotEqual(
                            Expression.Property(parameter, isDeletedProperty.PropertyInfo),
                            Expression.Constant(true, typeof(bool))
                        ),
                        parameter
                    );
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }
    }
}
