using Common.RequestWrapper;

namespace Application.Features.Expense.Commands
{
    public class RejectExpenseCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public string RejectionReason { get; set; }

        public RejectExpenseCommand()
        {
        }

        public RejectExpenseCommand(Guid id, string rejectionReason)
        {
            Id = id;
            RejectionReason = rejectionReason;
        }
    }
}