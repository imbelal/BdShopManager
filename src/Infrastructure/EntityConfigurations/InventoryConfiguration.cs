using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductId).HasColumnName("ProductId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.SupplierId).HasColumnName("SupplierId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName("Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.CostPerUnit).HasColumnName("CostPerUnit").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.TotalCost).HasColumnName("TotalCost").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Remark).HasColumnName("Remark").HasColumnType("nvarchar(max)").IsRequired(false);
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
        }
    }
}
