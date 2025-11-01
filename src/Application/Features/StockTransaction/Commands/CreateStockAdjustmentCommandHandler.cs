using Common.Exceptions;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.StockTransaction.Commands
{
    public class CreateStockAdjustmentCommandHandler : ICommandHandler<CreateStockAdjustmentCommand, Guid>
    {
        private readonly IStockAdjustmentRepository _stockAdjustmentRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public CreateStockAdjustmentCommandHandler(
            IStockAdjustmentRepository stockAdjustmentRepository,
            IReadOnlyApplicationDbContext context)
        {
            _stockAdjustmentRepository = stockAdjustmentRepository;
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(CreateStockAdjustmentCommand command, CancellationToken cancellationToken)
        {
            // Validate product exists
            Domain.Entities.Product? product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == command.ProductId && !x.IsDeleted, cancellationToken);

            if (product == null)
            {
                throw new BusinessLogicException("Product not found!");
            }

            // Validate quantity
            if (command.Quantity <= 0)
            {
                throw new BusinessLogicException("Quantity must be greater than zero!");
            }

            // Validate transaction type
            if (command.Type != StockTransactionType.IN && command.Type != StockTransactionType.OUT)
            {
                throw new BusinessLogicException("Invalid transaction type! Must be IN or OUT.");
            }

            // For OUT adjustments, ensure sufficient stock
            if (command.Type == StockTransactionType.OUT && product.StockQuantity < command.Quantity)
            {
                throw new BusinessLogicException($"Insufficient stock! Available: {product.StockQuantity}, Requested: {command.Quantity}");
            }

            // Create stock adjustment using factory method
            // This will raise StockAdjustmentCreatedEvent which will:
            // 1. Update product stock
            // 2. Create stock transaction record
            var stockAdjustment = StockAdjustment.Create(
                productId: command.ProductId,
                type: command.Type,
                quantity: command.Quantity,
                reason: command.Reason);

            _stockAdjustmentRepository.Add(stockAdjustment);
            await _stockAdjustmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(stockAdjustment.Id);
        }
    }
}
