using Common.RequestWrapper;

namespace Application.Features.SalesReturn.Commands
{
    public class DeleteSalesReturnCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }

        public DeleteSalesReturnCommand(Guid id)
        {
            Id = id;
        }
    }
}
