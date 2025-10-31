using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PurchaseId).HasColumnName("PurchaseId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.ProductId).HasColumnName("ProductId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName("Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.CostPerUnit).HasColumnName("CostPerUnit").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.TotalCost).HasColumnName("TotalCost").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();

            // Relationships
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Purchase)
                .WithMany(x => x.PurchaseItems)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
