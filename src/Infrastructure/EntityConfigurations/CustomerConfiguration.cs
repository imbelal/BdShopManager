using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FirstName).HasColumnName("FirstName").HasColumnType("nvarchar(100)").IsRequired();
            builder.Property(x => x.LastName).HasColumnName("LastName").HasColumnType("nvarchar(100)").IsRequired(false);
            builder.Property(x => x.Address).HasColumnName("Address").HasColumnType("nvarchar(200)").IsRequired(false);
            builder.Property(x => x.ContactNo).HasColumnName("ContactNo").HasColumnType("nvarchar(20)").IsRequired(false);
            builder.Property(x => x.Email).HasColumnName("Email").HasColumnType("nvarchar(70)").IsRequired(false);
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
        }
    }
}
