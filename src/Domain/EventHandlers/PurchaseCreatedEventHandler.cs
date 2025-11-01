using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class PurchaseCreatedEventHandler : INotificationHandler<PurchaseCreatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public PurchaseCreatedEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _context = context;
        }

        public async Task Handle(PurchaseCreatedEvent notification, CancellationToken cancellationToken)
        {
            Product? product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);

            if (product != null)
            {
                // Update average cost before increasing stock (order matters for calculation)
                product.UpdateAverageCost(notification.CostPerUnit, notification.Quantity);
                product.IncreaseStockQuantity(notification.Quantity);
                _productRepository.Update(product);

                // Create stock transaction record for Purchase
                var stockTransaction = StockTransaction.CreateInbound(
                    productId: notification.ProductId,
                    refType: StockReferenceType.Purchase,
                    refId: notification.PurchaseId,
                    quantity: notification.Quantity,
                    unitCost: notification.CostPerUnit,
                    transactionDate: DateTime.UtcNow);

                _stockTransactionRepository.Add(stockTransaction);
            }
        }
    }
}
