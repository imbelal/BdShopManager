using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Inventory.Commands
{
    public class DeleteInventoryCommandHandler : ICommandHandler<DeleteInventoryCommand, bool>
    {
        private readonly IInventoryRepository _inventoryRepository;

        public DeleteInventoryCommandHandler(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IResponse<bool>> Handle(DeleteInventoryCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Inventory? inventory = await _inventoryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (inventory == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Inventory not found!!");
            }

            inventory.Delete();
            _inventoryRepository.Update(inventory);
            await _inventoryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
