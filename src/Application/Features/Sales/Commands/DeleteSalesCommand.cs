using Common.RequestWrapper;

namespace Application.Features.Sales.Commands
{
    public class DeleteSalesCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }

        public DeleteSalesCommand(Guid id)
        {
            Id = id;
        }
    }
}
