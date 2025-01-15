using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Inventory.EventListeners
{
    public class UpdateStockEventHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IReadOnlyApplicationDbContext _readOnlyApplicationDbContext;
        public UpdateStockEventHandler(IInventoryRepository inventoryRepository,
            IReadOnlyApplicationDbContext readOnlyApplicationDbContext)
        {
            _inventoryRepository = inventoryRepository;
            _readOnlyApplicationDbContext = readOnlyApplicationDbContext;
        }
        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            Order order = await _readOnlyApplicationDbContext.Orders.Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == notification.OrderId, cancellationToken);

            if (order == null) return;

            Dictionary<Guid, int> productQtyDict = order.OrderDetails.ToDictionary(x => x.ProductId, y => y.Quantity);
            List<Domain.Entities.Inventory> inventories = await _inventoryRepository.GetByProductIdsAsync(productQtyDict.Keys.ToList(), cancellationToken);
            foreach (Domain.Entities.Inventory inventory in inventories)
            {
                inventory.DecreaseQuantity(productQtyDict[inventory.ProductId]);
                _inventoryRepository.Update(inventory);
            }
        }
    }
}
