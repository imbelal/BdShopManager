using Common.RequestWrapper;

namespace Application.Features.Supplier.Commands
{
    public class DeleteSupplierCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }

        public DeleteSupplierCommand(Guid id)
        {
            Id = id;
        }
    }
}
