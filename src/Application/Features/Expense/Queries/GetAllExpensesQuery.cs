using Application.Features.Expense.DTOs;
using Common.Pagination;
using Common.RequestWrapper;

namespace Application.Features.Expense.Queries
{
    public class GetAllExpensesQuery : IQuery<Pagination<ExpenseDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ExpenseType { get; set; }
        public int? Status { get; set; }
        public string? PaymentMethod { get; set; }

        public GetAllExpensesQuery()
        {
        }

        public GetAllExpensesQuery(int pageNumber, int pageSize, string? searchTerm = null,
                                 DateTime? startDate = null, DateTime? endDate = null,
                                 int? expenseType = null, int? status = null, string? paymentMethod = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchTerm = searchTerm;
            StartDate = startDate;
            EndDate = endDate;
            ExpenseType = expenseType;
            Status = status;
            PaymentMethod = paymentMethod;
        }
    }
}