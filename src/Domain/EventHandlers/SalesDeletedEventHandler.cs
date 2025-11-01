using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesDeletedEventHandler : INotificationHandler<SalesDeletedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesDeletedEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _context = context;
        }

        public async Task Handle(SalesDeletedEvent notification, CancellationToken cancellationToken)
        {
            // Restore stock for all products in the deleted order
            foreach (var salesItem in notification.SalesItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == salesItem.ProductId, cancellationToken);

                if (product != null)
                {
                    // Increase stock (reversing the original sales decrease)
                    product.IncreaseStockQuantity(salesItem.Quantity);
                    _productRepository.Update(product);

                    // Create stock transaction record for Sales Deletion (IN to reverse the original OUT)
                    var reversalTransaction = StockTransaction.CreateInbound(
                        productId: salesItem.ProductId,
                        refType: StockReferenceType.Sale,
                        refId: notification.SalesId,
                        quantity: salesItem.Quantity,
                        unitCost: product.CostPrice, // Use current average cost
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(reversalTransaction);
                }
            }
        }
    }
}
