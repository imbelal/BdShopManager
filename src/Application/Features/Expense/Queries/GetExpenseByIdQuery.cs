using Application.Features.Expense.DTOs;
using Common.RequestWrapper;

namespace Application.Features.Expense.Queries
{
    public class GetExpenseByIdQuery : IQuery<ExpenseDto>
    {
        public Guid Id { get; set; }

        public GetExpenseByIdQuery()
        {
        }

        public GetExpenseByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}