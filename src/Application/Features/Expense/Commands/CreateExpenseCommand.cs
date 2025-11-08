using Common.RequestWrapper;
using Domain.Enums;

namespace Application.Features.Expense.Commands
{
    public class CreateExpenseCommand : ICommand<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public DateTime ExpenseDate { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? ReceiptNumber { get; set; }

        public CreateExpenseCommand()
        {
        }

        public CreateExpenseCommand(string title, string description, decimal amount, string remarks,
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
    }
}