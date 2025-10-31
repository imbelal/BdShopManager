using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SupplierId).HasColumnName("SupplierId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.PurchaseDate).HasColumnName("PurchaseDate").HasColumnType("datetime2").IsRequired();
            builder.Property(x => x.TotalCost).HasColumnName("TotalCost").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Remark).HasColumnName("Remark").HasColumnType("nvarchar(max)").IsRequired(false);
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();

            // Relationships
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.PurchaseItems)
                .WithOne(x => x.Purchase)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
