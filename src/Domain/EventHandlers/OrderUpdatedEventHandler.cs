using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class OrderUpdatedEventHandler : INotificationHandler<OrderUpdatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public OrderUpdatedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task Handle(OrderUpdatedEvent notification, CancellationToken cancellationToken)
        {
            // Calculate stock adjustments needed
            // Step 1: Restore stock for products in old order details
            foreach (var oldDetail in notification.OldOrderDetails)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == oldDetail.ProductId, cancellationToken);

                if (product != null)
                {
                    product.IncreaseStockQuantity(oldDetail.Quantity);
                    _productRepository.Update(product);
                }
            }

            // Step 2: Decrease stock for products in new order details
            foreach (var newDetail in notification.NewOrderDetails)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == newDetail.ProductId, cancellationToken);

                if (product != null)
                {
                    product.DecreaseStockQuantity(newDetail.Quantity);
                    _productRepository.Update(product);
                }
            }
        }
    }
}
