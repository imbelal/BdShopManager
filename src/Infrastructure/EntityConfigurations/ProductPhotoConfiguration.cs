using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class ProductPhotoConfiguration : IEntityTypeConfiguration<ProductPhoto>
    {
        public void Configure(EntityTypeBuilder<ProductPhoto> builder)
        {
            builder.ToTable("ProductPhotos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.ProductId)
                .IsRequired();

            builder.Property(p => p.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.FileSize)
                .IsRequired();

            builder.Property(p => p.BlobUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.IsPrimary)
                .IsRequired();

            builder.Property(p => p.DisplayOrder)
                .IsRequired();

            // Configure relationship with Product
            builder.HasOne<Product>()
                .WithMany(p => p.ProductPhotos)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}