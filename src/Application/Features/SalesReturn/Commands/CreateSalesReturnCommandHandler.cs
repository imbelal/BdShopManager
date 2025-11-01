using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SalesReturn.Commands
{
    public class CreateSalesReturnCommandHandler : ICommandHandler<CreateSalesReturnCommand, SalesReturnDto>
    {
        private readonly ISalesReturnRepository _salesReturnRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public CreateSalesReturnCommandHandler(ISalesReturnRepository salesReturnRepository, IReadOnlyApplicationDbContext context)
        {
            _salesReturnRepository = salesReturnRepository;
            _context = context;
        }

        public async Task<IResponse<SalesReturnDto>> Handle(CreateSalesReturnCommand command, CancellationToken cancellationToken)
        {
            // Validate that the sales exists
            var sales = await _context.Sales
                .FirstOrDefaultAsync(x => x.Id == command.SalesId && !x.IsDeleted, cancellationToken);

            if (sales == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Sales not found!");
            }

            // Validate all products exist
            var productIds = command.SalesReturnItems.Select(item => item.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            foreach (var returnItem in command.SalesReturnItems)
            {
                if (!products.ContainsKey(returnItem.ProductId))
                {
                    throw new Common.Exceptions.BusinessLogicException($"Product with ID {returnItem.ProductId} not found!");
                }
            }

            // Create sales return with items
            var salesReturn = Domain.Entities.SalesReturn.CreateSalesReturnWithItems(
                command.SalesId,
                command.TotalRefundAmount,
                command.Remark,
                command.SalesReturnItems);

            // Generate and set unique return number
            var returnNumber = await GenerateReturnNumber(cancellationToken);
            salesReturn.SetReturnNumber(returnNumber);

            _salesReturnRepository.Add(salesReturn);
            await _salesReturnRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var salesReturnDto = new SalesReturnDto
            {
                Id = salesReturn.Id,
                ReturnNumber = salesReturn.ReturnNumber,
                SalesId = salesReturn.SalesId,
                SalesNumber = sales.SalesNumber,
                TotalRefundAmount = salesReturn.TotalRefundAmount,
                Remark = salesReturn.Remark,
                CreatedUtcDate = salesReturn.CreatedUtcDate.DateTime,
                SalesReturnItems = salesReturn.SalesReturnItems.Select(item => new SalesReturnItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductTitle = products[item.ProductId].Title,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    Reason = item.Reason
                }).ToList()
            };

            return Response.Success(salesReturnDto);
        }

        private async Task<string> GenerateReturnNumber(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            // Get the count of returns created today to determine the sequence number
            var returnsToday = await _context.SalesReturns
                .Where(r => r.CreatedUtcDate >= today && r.CreatedUtcDate < tomorrow)
                .CountAsync(cancellationToken);

            var sequenceNumber = returnsToday + 1;

            // Use domain entity method for return number formatting
            return Domain.Entities.SalesReturn.GenerateReturnNumber(today, sequenceNumber);
        }
    }
}
