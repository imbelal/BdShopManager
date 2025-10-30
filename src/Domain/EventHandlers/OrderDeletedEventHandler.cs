using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class OrderDeletedEventHandler : INotificationHandler<OrderDeletedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public OrderDeletedEventHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
        {
            // Restore stock for all products in the deleted order
            foreach (var orderDetail in notification.OrderDetails)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == orderDetail.ProductId, cancellationToken);

                if (product != null)
                {
                    product.IncreaseStockQuantity(orderDetail.Quantity);
                    _productRepository.Update(product);
                }
            }
        }
    }
}
