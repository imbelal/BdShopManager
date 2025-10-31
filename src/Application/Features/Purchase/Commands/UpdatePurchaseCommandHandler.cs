using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Purchase.Commands
{
    public class UpdatePurchaseCommandHandler : ICommandHandler<UpdatePurchaseCommand, bool>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public UpdatePurchaseCommandHandler(IPurchaseRepository purchaseRepository, IReadOnlyApplicationDbContext context)
        {
            _purchaseRepository = purchaseRepository;
            _context = context;
        }

        public async Task<IResponse<bool>> Handle(UpdatePurchaseCommand command, CancellationToken cancellationToken)
        {
            // Validate purchase exists
            Domain.Entities.Purchase? purchase = await _context.Purchases
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (purchase == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Purchase not found!!");
            }

            // Validate supplier exists
            Domain.Entities.Supplier? supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == command.SupplierId, cancellationToken);
            if (supplier == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Supplier not found!!");
            }

            // Update purchase through aggregate root
            purchase.Update(command.SupplierId, command.PurchaseDate, command.Remark);

            _purchaseRepository.Update(purchase);
            await _purchaseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
