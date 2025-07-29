using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class InventoryAddedEventHandler : INotificationHandler<InventoryAddedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public InventoryAddedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task Handle(InventoryAddedEvent notification, CancellationToken cancellationToken)
        {
            Product? product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);

            if (product != null)
            {
                product.IncreaseStockQuantity(notification.Quantity);
                _productRepository.Update(product);
            }
        }
    }
}
