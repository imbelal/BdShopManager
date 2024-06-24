using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace IntegrationTest
{
    /// <summary>
    /// Do adjustments to the model builder to ensure that our configuration work with a sqlite memory database we use for testing.
    /// </summary>
    public static class SQLiteModelBuilderService
    {
        public static void OnModelCreating(ModelBuilder builder)
        {
            foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
            {
                AdjustForDateTimeOffset(builder, entityType);
                AdjustForNvarcharMax(builder, entityType);
                AdjustForValueGeneratedOnAddOrUpdate(builder, entityType);
                AdjustGeography(builder, entityType);
            }
        }

        /// <summary>
        /// SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
        /// here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
        /// To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
        /// use the DateTimeOffsetToBinaryConverter
        /// Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
        /// This only supports millisecond precision, but should be sufficient for most use cases.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entityType"></param>
        private static void AdjustForDateTimeOffset(
            ModelBuilder builder,
            IMutableEntityType entityType
        )
        {
            IEnumerable<PropertyInfo> properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));
            foreach (PropertyInfo property in properties)
            {
                builder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(new DateTimeOffsetToBinaryConverter());
            }
        }

        /// <summary>
        /// SQLite has no support for nvarchar(max).
        /// So we simply use only nvarchar.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entityType"></param>
        private static void AdjustForNvarcharMax(
            ModelBuilder builder,
            IMutableEntityType entityType
        )
        {
            IEnumerable<IMutableProperty> properties = entityType.GetProperties().Where(p => p.GetColumnType() != null && p.GetColumnType().Contains("max"));
            foreach (IMutableProperty property in properties)
            {
                property.SetColumnType("nvarchar");
            }
        }

        /// <summary>
        /// SQLite has no support for ValueGeneratedOnAddOrUpdate for non primary columns.
        /// But we do not need auto increment for tests.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entityType"></param>
        private static void AdjustForValueGeneratedOnAddOrUpdate(
            ModelBuilder builder,
            IMutableEntityType entityType
        )
        {
            IEnumerable<IMutableProperty> properties = entityType.GetProperties().Where(p =>
                p.ValueGenerated != ValueGenerated.Never
            );
            foreach (IMutableProperty property in properties)
            {
                property.ValueGenerated = ValueGenerated.Never;
            }
        }

        private static void AdjustGeography(
            ModelBuilder builder,
            IMutableEntityType entityType
        )
        {
            IEnumerable<IMutableProperty> properties = entityType.GetProperties().Where(p => p.GetColumnType() != null && p.GetColumnType().Contains("geography"));
            foreach (IMutableProperty property in properties)
            {
                property.SetColumnType("point");
            }
        }
    }
}
