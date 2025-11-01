using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesReturnCreatedEventHandler : INotificationHandler<SalesReturnCreatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesReturnCreatedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task Handle(SalesReturnCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Increase stock for each product in the return (items are coming back to inventory)
            foreach (var returnItem in notification.SalesReturnItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == returnItem.ProductId, cancellationToken);

                if (product != null)
                {
                    product.IncreaseStockQuantity(returnItem.Quantity);
                    _productRepository.Update(product);
                }
            }
        }
    }
}
