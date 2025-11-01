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
            // Validate purchase exists and load with items
            Domain.Entities.Purchase? purchase = await _purchaseRepository.GetByIdAsync(command.Id, cancellationToken);
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

            // Validate all products exist in the new purchase items
            foreach (var item in command.PurchaseItems)
            {
                Domain.Entities.Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == item.ProductId, cancellationToken);
                if (product == null)
                {
                    throw new Common.Exceptions.BusinessLogicException($"Product with ID {item.ProductId} not found!!");
                }
            }

            // Update purchase with items through aggregate root - domain events will handle stock and cost adjustments
            purchase.UpdateWithItems(command.SupplierId, command.PurchaseDate, command.Remark, command.PurchaseItems);

            _purchaseRepository.Update(purchase);
            await _purchaseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
