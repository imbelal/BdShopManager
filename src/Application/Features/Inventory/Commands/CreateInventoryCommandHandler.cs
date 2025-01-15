using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Inventory.Commands
{
    public class CreateInventoryCommandHandler : ICommandHandler<CreateInventoryCommand, Guid>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public CreateInventoryCommandHandler(IInventoryRepository inventoryRepository, IReadOnlyApplicationDbContext context)
        {
            _inventoryRepository = inventoryRepository;
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(CreateInventoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Product? product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == command.ProductId, cancellationToken);
            if (product == null)
            {
                throw new Exception("Product not found!!");
            }

            Domain.Entities.Supplier? supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == command.SupplierId, cancellationToken);
            if (supplier == null)
            {
                throw new Exception("Supplier not found!!");
            }

            Domain.Entities.Inventory inventory = new(command.ProductId, command.SupplierId, command.Quantity, command.CostPerUnit, command.Quantity * command.CostPerUnit, command.Remark);
            _inventoryRepository.Add(inventory);
            await _inventoryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(inventory.Id);
        }
    }
}
