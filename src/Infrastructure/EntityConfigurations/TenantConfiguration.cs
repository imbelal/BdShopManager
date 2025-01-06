using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnName("Name").HasColumnType("nvarchar(200)").IsRequired();
            builder.Property(x => x.Address).HasColumnName("Address").HasColumnType("nvarchar(200)").IsRequired();
            builder.Property(x => x.PhoneNumber).HasColumnName("PhoneNumber").HasColumnType("nvarchar(20)").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
        }
    }
}
