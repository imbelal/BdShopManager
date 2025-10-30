using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Inventory.Commands
{
    public class UpdateInventoryCommandHandler : ICommandHandler<UpdateInventoryCommand, bool>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public UpdateInventoryCommandHandler(IInventoryRepository inventoryRepository, IReadOnlyApplicationDbContext context)
        {
            _inventoryRepository = inventoryRepository;
            _context = context;
        }

        public async Task<IResponse<bool>> Handle(UpdateInventoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Inventory? inventory = await _inventoryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (inventory == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Inventory not found!!");
            }

            Domain.Entities.Product? product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == command.ProductId, cancellationToken);
            if (product == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Product not found!!");
            }

            Domain.Entities.Supplier? supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == command.SupplierId, cancellationToken);
            if (supplier == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Supplier not found!!");
            }

            inventory.Update(command.ProductId, command.SupplierId, command.Quantity, command.CostPerUnit, command.Remark);
            _inventoryRepository.Update(inventory);
            await _inventoryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
