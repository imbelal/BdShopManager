using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class SalesConfiguration : IEntityTypeConfiguration<Sales>
    {
        public void Configure(EntityTypeBuilder<Sales> builder)
        {
            builder.ToTable("Sales");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SalesNumber).HasColumnName("SalesNumber");
            builder.Property(x => x.CustomerId).HasColumnName("CustomerId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.TotalPrice).HasColumnName("TotalPrice").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.DiscountPercentage).HasColumnName("DiscountPercentage").HasColumnType("decimal(5,2)").IsRequired().HasDefaultValue(0);
            builder.Property(x => x.TaxPercentage).HasColumnName("TaxPercentage").HasColumnType("decimal(5,2)").IsRequired().HasDefaultValue(0);
            builder.Property(x => x.TotalPaid).HasColumnName("TotalPaid").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Status).HasColumnName("Status").IsRequired().HasConversion<int>();
            builder.Property(x => x.Remark).HasColumnName("Remark").HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict); ;
        }
    }
}
