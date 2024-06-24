using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).HasColumnName("UserId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Token).HasColumnName("Token").HasColumnType("nvarchar(max)").IsRequired();
        }
    }
}
