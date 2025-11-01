using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesCancelledEventHandler : INotificationHandler<SalesCancelledEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesCancelledEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _context = context;
        }

        public async Task Handle(SalesCancelledEvent notification, CancellationToken cancellationToken)
        {
            // Restore stock for each product in the cancelled order
            foreach (var salesItem in notification.SalesItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == salesItem.ProductId, cancellationToken);

                if (product != null)
                {
                    product.IncreaseStockQuantity(salesItem.Quantity);
                    _productRepository.Update(product);

                    // Create stock transaction record for Sales Cancellation (IN transaction)
                    var stockTransaction = StockTransaction.CreateInbound(
                        productId: salesItem.ProductId,
                        refType: StockReferenceType.SalesCancellation,
                        refId: notification.SalesId,
                        quantity: salesItem.Quantity,
                        unitCost: product.CostPrice, // Use current average cost
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(stockTransaction);
                }
            }
        }
    }
}