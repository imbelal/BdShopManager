using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Supplier.Commands
{
    public class CreateSupplierCommandHandler : ICommandHandler<CreateSupplierCommand, Guid>
    {
        private readonly ISupplierRepository _supplierRepository;

        public CreateSupplierCommandHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IResponse<Guid>> Handle(CreateSupplierCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Supplier supplier = new(command.Name, command.Details, command.ContactNo);
            _supplierRepository.Add(supplier);
            await _supplierRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(supplier.Id);
        }
    }
}
