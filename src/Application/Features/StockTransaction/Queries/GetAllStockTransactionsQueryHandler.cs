using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.StockTransaction.Queries
{
    public class GetAllStockTransactionsQueryHandler : IQueryHandler<GetAllStockTransactionsQuery, Pagination<StockTransactionDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetAllStockTransactionsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<StockTransactionDto>>> Handle(GetAllStockTransactionsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.StockTransactions
                .Where(st => !st.IsDeleted);

            // Apply filters
            if (request.ProductId.HasValue)
            {
                query = query.Where(st => st.ProductId == request.ProductId.Value);
            }

            if (request.Type.HasValue)
            {
                query = query.Where(st => st.Type == request.Type.Value);
            }

            if (request.RefType.HasValue)
            {
                query = query.Where(st => st.RefType == request.RefType.Value);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(st => st.TransactionDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                // Include the entire end date
                var endOfDay = request.ToDate.Value.Date.AddDays(1);
                query = query.Where(st => st.TransactionDate < endOfDay);
            }

            var transactionsDtoQuery = query
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
                });

            var pagination = await new Pagination<StockTransactionDto>().CreateAsync(transactionsDtoQuery, request.PageNumber, request.PageSize, cancellationToken);

            if (pagination.Items.Count == 0)
                return Response.Fail<Pagination<StockTransactionDto>>("No stock transactions found!");

            return Response.Success(pagination);
        }
    }
}
