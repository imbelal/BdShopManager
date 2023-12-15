using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
    {
        public void Configure(EntityTypeBuilder<ErrorLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDateUtc).HasColumnName("CreatedDateUtc").HasColumnType("datetime2").IsRequired();
            builder.Property(x => x.Message).HasColumnName("Message").HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.Level).HasColumnName("Level").HasColumnType("nvarchar").HasMaxLength(10).IsRequired();
            builder.Property(x => x.Exception).HasColumnName("Exception").HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.StackTrace).HasColumnName("StackTrace").HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.Logger).HasColumnName("Logger").HasColumnType("nvarchar(max)").IsRequired();
        }
    }
}
