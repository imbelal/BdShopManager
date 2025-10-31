using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesDeletedEventHandler : INotificationHandler<SalesDeletedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesDeletedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task Handle(SalesDeletedEvent notification, CancellationToken cancellationToken)
        {
            // Restore stock for all products in the deleted order
            foreach (var salesItem in notification.SalesItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == salesItem.ProductId, cancellationToken);

                if (product != null)
                {
                    product.IncreaseStockQuantity(salesItem.Quantity);
                    _productRepository.Update(product);
                }
            }
        }
    }
}
