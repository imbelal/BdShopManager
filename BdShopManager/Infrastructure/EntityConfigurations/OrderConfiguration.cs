﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CustomerId).HasColumnName("CustomerId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.TotalPrice).HasColumnName("TotalPrice").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.TotalPaid).HasColumnName("TotalPaid").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Remark).HasColumnName("Remark").HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
        }
    }
}
