using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesReturnDeletedEventHandler : INotificationHandler<SalesReturnDeletedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesReturnDeletedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task Handle(SalesReturnDeletedEvent notification, CancellationToken cancellationToken)
        {
            // Decrease stock for each product when return is deleted (reverse the stock increase)
            foreach (var returnItem in notification.SalesReturnItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == returnItem.ProductId, cancellationToken);

                if (product != null)
                {
                    product.DecreaseStockQuantity(returnItem.Quantity);
                    _productRepository.Update(product);
                }
            }
        }
    }
}
