using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Purchase.Commands
{
    public class CancelPurchaseCommandHandler : ICommandHandler<CancelPurchaseCommand, bool>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public CancelPurchaseCommandHandler(IPurchaseRepository purchaseRepository, IReadOnlyApplicationDbContext context)
        {
            _purchaseRepository = purchaseRepository;
            _context = context;
        }

        public async Task<IResponse<bool>> Handle(CancelPurchaseCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Purchase? purchase = await _purchaseRepository.GetByIdAsync(command.Id, cancellationToken);
            if (purchase == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Purchase not found!");
            }

            // Check if purchase can be cancelled
            if (purchase.Status == PurchaseStatus.Cancelled)
            {
                throw new Common.Exceptions.BusinessLogicException("Purchase is already cancelled!");
            }

            // Cancel the purchase
            purchase.Cancel();
            _purchaseRepository.Update(purchase);
            await _purchaseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}