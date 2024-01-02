using Common.RequestWrapper;

namespace Application.Features.Customer.Commands
{
    public class DeleteCustomerCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }

        public DeleteCustomerCommand(Guid id)
        {
            Id = id;
        }
    }
}
