using Common.RequestWrapper;

namespace Application.Features.Expense.Commands
{
    public class MarkExpenseAsPaidCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }

        public MarkExpenseAsPaidCommand()
        {
        }

        public MarkExpenseAsPaidCommand(Guid id)
        {
            Id = id;
        }
    }
}