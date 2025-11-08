using Common.RequestWrapper;

namespace Application.Features.Expense.Queries
{
    public class GetExpensesPdfQuery : IQuery<byte[]>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}