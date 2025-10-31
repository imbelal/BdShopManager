using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Purchase.Commands
{
    public class CreatePurchaseCommandHandler : ICommandHandler<CreatePurchaseCommand, Guid>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public CreatePurchaseCommandHandler(IPurchaseRepository purchaseRepository, IReadOnlyApplicationDbContext context)
        {
            _purchaseRepository = purchaseRepository;
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(CreatePurchaseCommand command, CancellationToken cancellationToken)
        {
            // Validate supplier exists
            Domain.Entities.Supplier? supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == command.SupplierId, cancellationToken);
            if (supplier == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Supplier not found!!");
            }

            // Validate all products exist
            foreach (var item in command.PurchaseItems)
            {
                Domain.Entities.Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == item.ProductId, cancellationToken);
                if (product == null)
                {
                    throw new Common.Exceptions.BusinessLogicException($"Product with ID {item.ProductId} not found!!");
                }
            }

            // Create purchase aggregate - domain events will handle product updates
            Domain.Entities.Purchase purchase = new(command.SupplierId, command.PurchaseDate, command.Remark);

            // Add purchase items through the aggregate root
            foreach (var item in command.PurchaseItems)
            {
                purchase.AddPurchaseItem(item.ProductId, item.Quantity, item.CostPerUnit);
            }

            _purchaseRepository.Add(purchase);
            await _purchaseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(purchase.Id);
        }
    }
}
