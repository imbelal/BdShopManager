using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductId).HasColumnName("ProductId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Type).HasColumnName("Type").HasColumnType("int").IsRequired();
            builder.Property(x => x.RefType).HasColumnName("RefType").HasColumnType("int").IsRequired();
            builder.Property(x => x.RefId).HasColumnName("RefId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName("Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.UnitCost).HasColumnName("UnitCost").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.TotalCost).HasColumnName("TotalCost").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.TransactionDate).HasColumnName("TransactionDate").HasColumnType("datetime2").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId);

            // Index for performance on common queries
            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.RefId);
            builder.HasIndex(x => x.TransactionDate);
        }
    }
}
