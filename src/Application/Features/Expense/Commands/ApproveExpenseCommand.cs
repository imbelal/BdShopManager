using Common.RequestWrapper;

namespace Application.Features.Expense.Commands
{
    public class ApproveExpenseCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }

        public ApproveExpenseCommand()
        {
        }

        public ApproveExpenseCommand(Guid id)
        {
            Id = id;
        }
    }
}