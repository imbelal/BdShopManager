using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public OrderCreatedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Decrease stock for each product in the order
            foreach (var orderDetail in notification.OrderDetails)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == orderDetail.ProductId, cancellationToken);

                if (product != null)
                {
                    product.DecreaseStockQuantity(orderDetail.Quantity);
                    _productRepository.Update(product);
                }
            }
        }
    }
}
