using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesCreatedEventHandler : INotificationHandler<SalesCreatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesCreatedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task Handle(SalesCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Decrease stock for each product in the order
            foreach (var salesItem in notification.SalesItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == salesItem.ProductId, cancellationToken);

                if (product != null)
                {
                    product.DecreaseStockQuantity(salesItem.Quantity);
                    _productRepository.Update(product);
                }
            }
        }
    }
}
