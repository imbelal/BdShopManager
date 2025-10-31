using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class SalesItemConfiguration : IEntityTypeConfiguration<SalesItem>
    {
        public void Configure(EntityTypeBuilder<SalesItem> builder)
        {
            builder.ToTable("SalesItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SalesId).HasColumnName("SalesId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.ProductId).HasColumnName("ProductId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Unit).HasColumnName("Unit").HasColumnType("int").IsRequired();
            builder.Property(x => x.UnitPrice).HasColumnName("UnitPrice").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Quantity).HasColumnName("Quantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.TotalPrice).HasColumnName("TotalPrice").HasColumnType("decimal(18,2)").IsRequired();
            builder.HasOne(x => x.Sales).WithMany(x => x.SalesItems).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict); ;
        }
    }
}
