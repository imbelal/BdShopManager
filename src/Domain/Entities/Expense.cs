using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Enums;

namespace Domain.Entities
{
    public class Expense : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public DateTime ExpenseDate { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public ExpenseStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? ReceiptNumber { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Tenant Tenant { get; set; }

        public Expense() : base()
        {
        }

        public Expense(string title, string description, decimal amount, string remarks,
                      DateTime expenseDate, ExpenseType expenseType, PaymentMethod paymentMethod,
                      string? receiptNumber = null) : base(Guid.NewGuid())
        {
            Title = title;
            Description = description;
            Amount = amount;
            Remarks = remarks;
            ExpenseDate = expenseDate;
            ExpenseType = expenseType;
            PaymentMethod = paymentMethod;
            ReceiptNumber = receiptNumber;
            Status = ExpenseStatus.Pending;
        }

        public void Update(string title, string description, decimal amount, string remarks,
                          DateTime expenseDate, ExpenseType expenseType, PaymentMethod paymentMethod,
                          string? receiptNumber = null)
        {
            Title = title;
            Description = description;
            Amount = amount;
            Remarks = remarks;
            ExpenseDate = expenseDate;
            ExpenseType = expenseType;
            PaymentMethod = paymentMethod;
            ReceiptNumber = receiptNumber;
        }

        public void Approve(string approvedBy)
        {
            Status = ExpenseStatus.Approved;
            ApprovedBy = approvedBy;
            ApprovedDate = DateTime.UtcNow;
        }

        public void Reject(string approvedBy, string rejectionReason)
        {
            Status = ExpenseStatus.Rejected;
            ApprovedBy = approvedBy;
            ApprovedDate = DateTime.UtcNow;
            Remarks = string.IsNullOrEmpty(Remarks)
                ? $"Rejected: {rejectionReason}"
                : $"{Remarks}\n\nRejected: {rejectionReason}";
        }

        public void MarkAsPaid()
        {
            if (Status != ExpenseStatus.Approved)
            {
                throw new Common.Exceptions.BusinessLogicException("Only approved expenses can be marked as paid.");
            }

            Status = ExpenseStatus.Paid;
            PaidDate = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

        public bool CanBeEdited()
        {
            return Status == ExpenseStatus.Pending || Status == ExpenseStatus.Rejected;
        }

        public bool CanBeApproved()
        {
            return Status == ExpenseStatus.Pending;
        }

        public bool CanBeRejected()
        {
            return Status == ExpenseStatus.Pending;
        }

        public bool CanBePaid()
        {
            return Status == ExpenseStatus.Approved;
        }
    }
}