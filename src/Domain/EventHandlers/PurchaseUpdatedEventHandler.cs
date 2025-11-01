using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class PurchaseUpdatedEventHandler : INotificationHandler<PurchaseUpdatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public PurchaseUpdatedEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _context = context;
        }

        public async Task Handle(PurchaseUpdatedEvent notification, CancellationToken cancellationToken)
        {
            // Step 1: Reverse old purchase items (decrease stock and create reversal stock transactions)
            foreach (var oldItem in notification.OldPurchaseItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == oldItem.ProductId, cancellationToken);

                if (product != null)
                {
                    // Decrease stock to reverse the original purchase increase
                    product.DecreaseStockQuantity(oldItem.Quantity);
                    _productRepository.Update(product);

                    // Create reversal stock transaction (OUT to reverse the original IN)
                    var reversalTransaction = StockTransaction.CreateOutbound(
                        productId: oldItem.ProductId,
                        refType: StockReferenceType.Purchase,
                        refId: notification.PurchaseId,
                        quantity: oldItem.Quantity,
                        unitCost: oldItem.CostPerUnit,
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(reversalTransaction);
                }
            }

            // Step 2: Apply new purchase items (increase stock, update average cost, and create new stock transactions)
            foreach (var newItem in notification.NewPurchaseItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == newItem.ProductId, cancellationToken);

                if (product != null)
                {
                    // Update average cost before increasing stock
                    product.UpdateAverageCost(newItem.CostPerUnit, newItem.Quantity);
                    product.IncreaseStockQuantity(newItem.Quantity);
                    _productRepository.Update(product);

                    // Create new stock transaction (IN for purchase)
                    var newTransaction = StockTransaction.CreateInbound(
                        productId: newItem.ProductId,
                        refType: StockReferenceType.Purchase,
                        refId: notification.PurchaseId,
                        quantity: newItem.Quantity,
                        unitCost: newItem.CostPerUnit,
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(newTransaction);
                }
            }
        }
    }
}
