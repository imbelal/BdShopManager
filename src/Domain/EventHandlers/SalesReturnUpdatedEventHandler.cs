using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesReturnUpdatedEventHandler : INotificationHandler<SalesReturnUpdatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesReturnUpdatedEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _context = context;
        }

        public async Task Handle(SalesReturnUpdatedEvent notification, CancellationToken cancellationToken)
        {
            // Step 1: Reverse old return items (decrease stock and create reversal stock transactions)
            foreach (var oldReturnItem in notification.OldReturnItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == oldReturnItem.ProductId, cancellationToken);

                if (product != null)
                {
                    // Decrease stock (reversing the original return increase)
                    product.DecreaseStockQuantity(oldReturnItem.Quantity);
                    _productRepository.Update(product);

                    // Create reversal stock transaction (OUT to reverse the original IN)
                    var reversalTransaction = StockTransaction.CreateOutbound(
                        productId: oldReturnItem.ProductId,
                        refType: StockReferenceType.SalesReturn,
                        refId: notification.SalesReturnId,
                        quantity: oldReturnItem.Quantity,
                        unitCost: product.CostPrice, // Use current average cost
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(reversalTransaction);
                }
            }

            // Step 2: Apply new return items (increase stock and create new stock transactions)
            foreach (var newReturnItem in notification.NewReturnItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == newReturnItem.ProductId, cancellationToken);

                if (product != null)
                {
                    // Increase stock (items being returned to inventory)
                    product.IncreaseStockQuantity(newReturnItem.Quantity);
                    _productRepository.Update(product);

                    // Create new stock transaction (IN for return)
                    var newTransaction = StockTransaction.CreateInbound(
                        productId: newReturnItem.ProductId,
                        refType: StockReferenceType.SalesReturn,
                        refId: notification.SalesReturnId,
                        quantity: newReturnItem.Quantity,
                        unitCost: product.CostPrice, // Use current average cost
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(newTransaction);
                }
            }
        }
    }
}