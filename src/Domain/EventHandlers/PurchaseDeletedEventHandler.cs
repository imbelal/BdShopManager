using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class PurchaseDeletedEventHandler : INotificationHandler<PurchaseDeletedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public PurchaseDeletedEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _context = context;
        }

        public async Task Handle(PurchaseDeletedEvent notification, CancellationToken cancellationToken)
        {
            // Reverse all purchase items - decrease stock and create reversal transactions
            foreach (var purchaseItem in notification.PurchaseItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == purchaseItem.ProductId, cancellationToken);

                if (product != null)
                {
                    // Decrease stock (reversing the original purchase increase)
                    product.DecreaseStockQuantity(purchaseItem.Quantity);
                    _productRepository.Update(product);

                    // Create reversal stock transaction (OUT to reverse the original IN)
                    var reversalTransaction = StockTransaction.CreateOutbound(
                        productId: purchaseItem.ProductId,
                        refType: StockReferenceType.Purchase,
                        refId: notification.PurchaseId,
                        quantity: purchaseItem.Quantity,
                        unitCost: purchaseItem.CostPerUnit,
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(reversalTransaction);
                }
            }

            // Note: Average cost reversal is complex and would require tracking historical costs.
            // For now, we maintain the current average cost as it reflects the weighted average
            // of all historical purchases. Future purchases will adjust the average naturally.
        }
    }
}
