using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Supplier.Commands
{
    public class DeleteSupplierCommandHandler : ICommandHandler<DeleteSupplierCommand, Guid>
    {
        private readonly ISupplierRepository _supplierRepository;

        public DeleteSupplierCommandHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }
        public async Task<IResponse<Guid>> Handle(DeleteSupplierCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Supplier supplier = await _supplierRepository.GetByIdAsync(command.Id, cancellationToken);
            if (supplier == null) throw new Exception("Supplier not found!!");

            _supplierRepository.Remove(supplier);
            await _supplierRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(supplier.Id);
        }
    }
}
