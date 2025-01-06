using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Username).HasColumnName("Username").HasColumnType("nvarchar(70)").HasMaxLength(70).IsRequired();
            builder.Property(x => x.PasswordHash).HasColumnName("PasswordHash").HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.Email).HasColumnName("Email").HasColumnType("nvarchar(70)").HasMaxLength(70).IsRequired();
            builder.Property(x => x.FirstName).HasColumnName("FirstName").HasColumnType("nvarchar(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.LastName).HasColumnName("LastName").HasColumnType("nvarchar(50)").HasMaxLength(50).IsRequired();
            builder.Property(x => x.UserRoleId).HasColumnName("UserRoleId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.HasOne(x => x.UserRole).WithMany().HasForeignKey(x => x.UserRoleId);
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId);
        }
    }
}
