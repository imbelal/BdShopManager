using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class SalesReturnItemConfiguration : IEntityTypeConfiguration<SalesReturnItem>
    {
        public void Configure(EntityTypeBuilder<SalesReturnItem> builder)
        {
            builder.ToTable("SalesReturnItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SalesReturnId).HasColumnName("SalesReturnId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.ProductId).HasColumnName("ProductId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Unit).HasColumnName("Unit").HasColumnType("int").IsRequired();
            builder.Property(x => x.UnitPrice).HasColumnName("UnitPrice").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName("Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalPrice).HasColumnName("TotalPrice").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Reason).HasColumnName("Reason").HasColumnType("nvarchar(500)").IsRequired();
            builder.HasOne(x => x.SalesReturn).WithMany(x => x.SalesReturnItems).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
