using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.StockTransaction.Queries
{
    public class GetStockTransactionsByProductIdQueryHandler : IQueryHandler<GetStockTransactionsByProductIdQuery, List<StockTransactionDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetStockTransactionsByProductIdQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<StockTransactionDto>>> Handle(GetStockTransactionsByProductIdQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _context.StockTransactions
                .Where(st => !st.IsDeleted && st.ProductId == request.ProductId)
                .Join(_context.Products, st => st.ProductId, p => p.Id, (st, p) => new { StockTransaction = st, Product = p })
                .OrderByDescending(x => x.StockTransaction.TransactionDate)
                .Select(x => new StockTransactionDto
                {
                    Id = x.StockTransaction.Id,
                    ProductId = x.StockTransaction.ProductId,
                    ProductName = x.Product.Title,
                    Type = x.StockTransaction.Type,
                    TypeName = x.StockTransaction.Type.ToString(),
                    RefType = x.StockTransaction.RefType,
                    RefTypeName = x.StockTransaction.RefType.ToString(),
                    RefId = x.StockTransaction.RefId,
                    Quantity = x.StockTransaction.Quantity,
                    UnitCost = x.StockTransaction.UnitCost,
                    TotalCost = x.StockTransaction.TotalCost,
                    TransactionDate = x.StockTransaction.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    CreatedBy = x.StockTransaction.CreatedBy,
                    CreatedDate = x.StockTransaction.CreatedUtcDate.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync(cancellationToken);

            if (transactions.Count == 0)
                return Response.Fail<List<StockTransactionDto>>("No stock transactions found for this product!");

            return Response.Success(transactions);
        }
    }
}
