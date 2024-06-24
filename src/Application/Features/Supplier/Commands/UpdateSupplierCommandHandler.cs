using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Supplier.Commands
{
    public class UpdateSupplierCommandHandler : ICommandHandler<UpdateSupplierCommand, Guid>
    {
        private readonly ISupplierRepository _supplierRepository;

        public UpdateSupplierCommandHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IResponse<Guid>> Handle(UpdateSupplierCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Supplier supplier = await _supplierRepository.GetByIdAsync(command.Id);
            if (supplier == null) throw new Exception("Supplier not found!!");

            supplier.Update(command.Name, command.Details, command.ContactNo);
            _supplierRepository.Update(supplier);
            await _supplierRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(supplier.Id);
        }
    }
}
