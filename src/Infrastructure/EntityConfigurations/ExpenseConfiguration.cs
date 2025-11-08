using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Title).HasColumnName("Title").HasColumnType("nvarchar(200)").IsRequired();
            builder.Property(x => x.Description).HasColumnName("Description").HasColumnType("nvarchar(500)");
            builder.Property(x => x.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Remarks).HasColumnName("Remarks").HasColumnType("nvarchar(1000)");
            builder.Property(x => x.ExpenseDate).HasColumnName("ExpenseDate").HasColumnType("datetime2").IsRequired();
            builder.Property(x => x.ExpenseType).HasColumnName("ExpenseType").HasColumnType("int").IsRequired();
            builder.Property(x => x.Status).HasColumnName("Status").HasColumnType("int").IsRequired();
            builder.Property(x => x.PaymentMethod).HasColumnName("PaymentMethod").HasColumnType("int").IsRequired();
            builder.Property(x => x.ReceiptNumber).HasColumnName("ReceiptNumber").HasColumnType("nvarchar(100)");
            builder.Property(x => x.PaidDate).HasColumnName("PaidDate").HasColumnType("datetime2");
            builder.Property(x => x.ApprovedBy).HasColumnName("ApprovedBy").HasColumnType("nvarchar(200)");
            builder.Property(x => x.ApprovedDate).HasColumnName("ApprovedDate").HasColumnType("datetime2");
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.Property(x => x.TenantId).HasColumnName("TenantId").HasColumnType("uniqueidentifier").IsRequired();

            // Relationships
            builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId);

            // Indexes
            builder.HasIndex(x => new { x.TenantId, x.ExpenseDate });
            builder.HasIndex(x => new { x.TenantId, x.Status });
            builder.HasIndex(x => new { x.TenantId, x.ExpenseType });
            builder.HasIndex(x => x.ReceiptNumber).IsUnique();
        }
    }
}