using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesReturnDeletedEventHandler : INotificationHandler<SalesReturnDeletedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesReturnDeletedEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
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
                    // Decrease stock (reversing the original return increase)
                    product.DecreaseStockQuantity(returnItem.Quantity);
                    _productRepository.Update(product);

                    // Create stock transaction record for Sales Return Deletion (OUT to reverse the original IN)
                    var reversalTransaction = StockTransaction.CreateOutbound(
                        productId: returnItem.ProductId,
                        refType: StockReferenceType.SalesReturn,
                        refId: notification.SalesReturnId,
                        quantity: returnItem.Quantity,
                        unitCost: product.CostPrice, // Use current average cost
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(reversalTransaction);
                }
            }
        }
    }
}
