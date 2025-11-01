using Common.RequestWrapper;

namespace Application.Features.Sales.Commands
{
    public class CancelSalesCommand : ICommand<bool>
    {
        public Guid Id { get; set; }

        public CancelSalesCommand(Guid id)
        {
            Id = id;
        }
    }
}