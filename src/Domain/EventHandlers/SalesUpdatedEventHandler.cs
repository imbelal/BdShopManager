using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesUpdatedEventHandler : INotificationHandler<SalesUpdatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesUpdatedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task Handle(SalesUpdatedEvent notification, CancellationToken cancellationToken)
        {
            // Calculate stock adjustments needed
            // Step 1: Restore stock for products in old order details
            foreach (var oldItem in notification.OldSalesItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == oldItem.ProductId, cancellationToken);

                if (product != null)
                {
                    product.IncreaseStockQuantity(oldItem.Quantity);
                    _productRepository.Update(product);
                }
            }

            // Step 2: Decrease stock for products in new order details
            foreach (var newItem in notification.NewSalesItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == newItem.ProductId, cancellationToken);

                if (product != null)
                {
                    product.DecreaseStockQuantity(newItem.Quantity);
                    _productRepository.Update(product);
                }
            }
        }
    }
}
