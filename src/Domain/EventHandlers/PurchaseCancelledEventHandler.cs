using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class PurchaseCancelledEventHandler : INotificationHandler<PurchaseCancelledEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public PurchaseCancelledEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _context = context;
        }

        public async Task Handle(PurchaseCancelledEvent notification, CancellationToken cancellationToken)
        {
            // Reduce stock for each product in the cancelled purchase
            foreach (var purchaseItem in notification.PurchaseItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == purchaseItem.ProductId, cancellationToken);

                if (product != null)
                {
                    // Decrease stock quantity (OUT transaction)
                    product.DecreaseStockQuantity(purchaseItem.Quantity);
                    _productRepository.Update(product);

                    // Create stock transaction record for Purchase Cancellation (OUT transaction)
                    var stockTransaction = StockTransaction.CreateOutbound(
                        productId: purchaseItem.ProductId,
                        refType: StockReferenceType.PurchaseCancellation,
                        refId: notification.PurchaseId,
                        quantity: purchaseItem.Quantity,
                        unitCost: purchaseItem.CostPerUnit, // Use original purchase cost
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(stockTransaction);
                }
            }
        }
    }
}