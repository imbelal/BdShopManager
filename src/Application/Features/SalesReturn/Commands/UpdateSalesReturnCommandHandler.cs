using Common.Exceptions;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SalesReturn.Commands
{
    public class UpdateSalesReturnCommandHandler : ICommandHandler<UpdateSalesReturnCommand, Guid>
    {
        private readonly ISalesReturnRepository _salesReturnRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public UpdateSalesReturnCommandHandler(
            ISalesReturnRepository salesReturnRepository,
            IReadOnlyApplicationDbContext context)
        {
            _salesReturnRepository = salesReturnRepository;
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(UpdateSalesReturnCommand command, CancellationToken cancellationToken)
        {
            // Get the existing sales return
            Domain.Entities.SalesReturn? existingReturn = await _context.SalesReturns
                .Include(sr => sr.SalesReturnItems)
                .FirstOrDefaultAsync(x => x.Id == command.Id && !x.IsDeleted, cancellationToken);

            if (existingReturn == null)
            {
                throw new BusinessLogicException("Sales return not found!");
            }

            // Convert command items to DTOs for the Update method
            var updatedItems = command.SalesReturnItems.Select(item => new SalesReturnItemDetailsDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Unit = item.Unit,
                UnitPrice = item.UnitPrice,
                Reason = item.Reason
            }).ToList();

            // Validate total refund matches return items
            decimal calculatedTotal = updatedItems.Sum(item => item.Quantity * item.UnitPrice);
            if (Math.Abs(calculatedTotal - command.TotalRefundAmount) > 0.01m) // Allow for minor rounding differences
            {
                throw new BusinessLogicException($"Total refund amount mismatch. Provided: {command.TotalRefundAmount:N2}, Calculated from return items: {calculatedTotal:N2}");
            }

            // Update the sales return
            existingReturn.Update(command.TotalRefundAmount, command.Remark, updatedItems);
            _salesReturnRepository.Update(existingReturn);
            await _salesReturnRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(existingReturn.Id);
        }
    }
}