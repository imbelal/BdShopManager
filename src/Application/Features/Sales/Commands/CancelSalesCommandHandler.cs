using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sales.Commands
{
    public class CancelSalesCommandHandler : ICommandHandler<CancelSalesCommand, bool>
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public CancelSalesCommandHandler(ISalesRepository salesRepository, IReadOnlyApplicationDbContext context)
        {
            _salesRepository = salesRepository;
            _context = context;
        }

        public async Task<IResponse<bool>> Handle(CancelSalesCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Sales? sales = await _salesRepository.GetByIdAsync(command.Id, cancellationToken);
            if (sales == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Sales not found!");
            }

            // Check if sales can be cancelled
            if (sales.Status == Domain.Enums.SalesStatus.Paid)
            {
                throw new Common.Exceptions.BusinessLogicException("Cannot cancel sales that is fully paid!");
            }

            if (sales.Status == Domain.Enums.SalesStatus.Cancelled)
            {
                throw new Common.Exceptions.BusinessLogicException("Sales is already cancelled!");
            }

            // Cancel the sales
            sales.Cancel();
            _salesRepository.Update(sales);
            await _salesRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}