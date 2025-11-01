using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class SalesReturnCreatedEventHandler : INotificationHandler<SalesReturnCreatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public SalesReturnCreatedEventHandler(
            IProductRepository productRepository,
            IStockTransactionRepository stockTransactionRepository,
            IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _context = context;
        }

        public async Task Handle(SalesReturnCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Increase stock for each product in the return (items are coming back to inventory)
            foreach (var returnItem in notification.SalesReturnItems)
            {
                Product? product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == returnItem.ProductId, cancellationToken);

                if (product != null)
                {
                    product.IncreaseStockQuantity(returnItem.Quantity);
                    _productRepository.Update(product);

                    // Get the actual sales return item to get the unit price
                    var salesReturnItemEntity = await _context.SalesReturnItems
                        .FirstOrDefaultAsync(x => x.SalesReturnId == notification.SalesReturnId && x.ProductId == returnItem.ProductId, cancellationToken);

                    // Create stock transaction record for Sales Return
                    var stockTransaction = StockTransaction.CreateInbound(
                        productId: returnItem.ProductId,
                        refType: StockReferenceType.SalesReturn,
                        refId: notification.SalesReturnId,
                        quantity: returnItem.Quantity,
                        unitCost: salesReturnItemEntity?.UnitPrice ?? product.CostPrice, // Use the original sale price or fall back to cost price
                        transactionDate: DateTime.UtcNow);

                    _stockTransactionRepository.Add(stockTransaction);
                }
            }
        }
    }
}
