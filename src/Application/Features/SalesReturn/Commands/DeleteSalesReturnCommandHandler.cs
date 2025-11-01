using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.SalesReturn.Commands
{
    public class DeleteSalesReturnCommandHandler : ICommandHandler<DeleteSalesReturnCommand, Guid>
    {
        private readonly ISalesReturnRepository _salesReturnRepository;

        public DeleteSalesReturnCommandHandler(ISalesReturnRepository salesReturnRepository)
        {
            _salesReturnRepository = salesReturnRepository;
        }

        public async Task<IResponse<Guid>> Handle(DeleteSalesReturnCommand command, CancellationToken cancellationToken)
        {
            var salesReturn = await _salesReturnRepository.GetByIdAsync(command.Id, cancellationToken);
            if (salesReturn == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Sales return not found!");
            }

            salesReturn.Delete();
            _salesReturnRepository.Update(salesReturn);
            await _salesReturnRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(salesReturn.Id);
        }
    }
}
