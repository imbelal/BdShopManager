using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Sales.Commands
{
    public class DeleteSalesCommandHandler : ICommandHandler<DeleteSalesCommand, Guid>
    {
        private readonly ISalesRepository _salesRepository;

        public DeleteSalesCommandHandler(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }
        public async Task<IResponse<Guid>> Handle(DeleteSalesCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Sales? sales = await _salesRepository.GetByIdAsync(command.Id, cancellationToken);
            if (sales == null) throw new Common.Exceptions.BusinessLogicException("Sales not found!!");

            sales.Delete();
            _salesRepository.Update(sales);
            await _salesRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(sales.Id);
        }
    }
}
