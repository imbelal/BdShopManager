using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class SalesReturnConfiguration : IEntityTypeConfiguration<SalesReturn>
    {
        public void Configure(EntityTypeBuilder<SalesReturn> builder)
        {
            builder.ToTable("SalesReturns");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ReturnNumber).HasColumnName("ReturnNumber");
            builder.Property(x => x.SalesId).HasColumnName("SalesId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.TotalRefundAmount).HasColumnName("TotalRefundAmount").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Remark).HasColumnName("Remark").HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
