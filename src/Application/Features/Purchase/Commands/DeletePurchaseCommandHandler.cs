using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Purchase.Commands
{
    public class DeletePurchaseCommandHandler : ICommandHandler<DeletePurchaseCommand, bool>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public DeletePurchaseCommandHandler(IPurchaseRepository purchaseRepository, IReadOnlyApplicationDbContext context)
        {
            _purchaseRepository = purchaseRepository;
            _context = context;
        }

        public async Task<IResponse<bool>> Handle(DeletePurchaseCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Purchase? purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (purchase == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Purchase not found!!");
            }

            // Soft delete through aggregate root
            purchase.Delete();

            _purchaseRepository.Update(purchase);
            await _purchaseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
