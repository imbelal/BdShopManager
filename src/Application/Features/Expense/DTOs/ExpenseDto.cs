namespace Application.Features.Expense.DTOs
{
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int ExpenseType { get; set; }
        public string ExpenseTypeName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; }
        public string? ReceiptNumber { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? ApprovedByUserName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserName { get; set; }
    }
}