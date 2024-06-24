using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnName("Name").HasColumnType("nvarchar(200)").IsRequired();
            builder.Property(x => x.Details).HasColumnName("Details").HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.ContactNo).HasColumnName("ContactNo").HasColumnType("nvarchar(20)").IsRequired();
        }
    }
}
