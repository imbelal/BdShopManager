using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
    {
        public void Configure(EntityTypeBuilder<StockAdjustment> builder)
        {
            builder.ToTable("StockAdjustments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                .HasColumnName("ProductId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(x => x.Type)
                .HasColumnName("Type")
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasColumnName("Quantity")
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasColumnName("Reason")
                .HasColumnType("nvarchar(500)")
                .IsRequired();

            builder.Property(x => x.AdjustmentDate)
                .HasColumnName("AdjustmentDate")
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .HasColumnName("IsDeleted")
                .HasColumnType("bit")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.TenantId)
                .HasColumnName("TenantId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index for querying adjustments by product
            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.AdjustmentDate);
        }
    }
}
