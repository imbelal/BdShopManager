using Common.RequestWrapper;

namespace Application.Features.Expense.Commands
{
    public class DeleteExpenseCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }

        public DeleteExpenseCommand()
        {
        }

        public DeleteExpenseCommand(Guid id)
        {
            Id = id;
        }
    }
}