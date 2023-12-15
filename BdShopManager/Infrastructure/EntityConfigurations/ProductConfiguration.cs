using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasColumnName("Title").HasColumnType("nvarchar(200)").IsRequired();
            builder.Property(x => x.Description).HasColumnName("Description").HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.StockQuantity).HasColumnName("StockQuantity").HasColumnType("int").IsRequired();
            builder.Property(x => x.Unit).HasColumnName("Unit").HasColumnType("int").IsRequired();
            builder.Property(x => x.CategoryId).HasColumnName("CategoryId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Status).HasColumnName("Status").HasColumnType("tinyint").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.HasMany(x => x.PostTags).WithOne(x => x.Post).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade).IsRequired(false);
        }
    }
}
