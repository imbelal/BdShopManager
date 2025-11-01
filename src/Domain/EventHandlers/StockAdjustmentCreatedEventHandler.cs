using Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class StockAdjustmentCreatedEventHandler : INotificationHandler<StockAdjustmentCreatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public StockAdjustmentCreatedEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _context = context;
        }

        public async Task Handle(StockAdjustmentCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Get the product
            Product? product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);

            if (product == null)
            {
                throw new BusinessLogicException("Product not found!");
            }

            // For OUT adjustments, ensure sufficient stock
            if (notification.Type == StockTransactionType.OUT && product.StockQuantity < notification.Quantity)
            {
                throw new BusinessLogicException($"Insufficient stock! Available: {product.StockQuantity}, Requested: {notification.Quantity}");
            }

            // Update product stock based on adjustment type
            if (notification.Type == StockTransactionType.IN)
            {
                product.IncreaseStockQuantity(notification.Quantity);
            }
            else if (notification.Type == StockTransactionType.OUT)
            {
                product.DecreaseStockQuantity(notification.Quantity);
            }

            _productRepository.Update(product);

            // Create stock transaction record
            var stockTransaction = StockTransaction.CreateInbound(
                productId: notification.ProductId,
                refType: StockReferenceType.Adjustment,
                refId: notification.AdjustmentId,
                quantity: notification.Quantity,
                unitCost: product.CostPrice,
                transactionDate: DateTime.UtcNow);

            // Override with correct type (IN or OUT) based on adjustment
            var transaction = new StockTransaction(
                productId: notification.ProductId,
                type: notification.Type,
                refType: StockReferenceType.Adjustment,
                refId: notification.AdjustmentId,
                quantity: notification.Quantity,
                unitCost: product.CostPrice,
                transactionDate: DateTime.UtcNow);

            _stockTransactionRepository.Add(transaction);
        }
    }
}
