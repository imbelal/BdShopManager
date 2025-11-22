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
            if (sales.Status == Domain.Enums.SalesStatus.Cancelled)
            {
                throw new Common.Exceptions.BusinessLogicException("Sales is already cancelled!");
            }

            // Create reversal payment if any payment was made
            if (sales.TotalPaid > 0)
            {
                // Create a negative payment to reverse the amount
                var reversalAmount = -sales.TotalPaid; // Negative amount for reversal
                var reversalRemark = $"Payment reversal for cancelled sales {sales.SalesNumber}";

                // Add the reversal payment using the new AddPaymentReversal method
                sales.AddPaymentReversal(reversalAmount, "Reversal", reversalRemark);
            }

            // Cancel the sales
            sales.Cancel();
            _salesRepository.Update(sales);
            await _salesRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}