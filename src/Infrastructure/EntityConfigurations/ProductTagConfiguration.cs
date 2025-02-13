﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
    {
        public void Configure(EntityTypeBuilder<ProductTag> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductId).HasColumnName("ProductId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.TagId).HasColumnName("TagId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict); ;
        }
    }
}
