using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SalesReturn.Queries
{
    public class GetAllSalesReturnsQueryHandler : IQueryHandler<GetAllSalesReturnsQuery, Pagination<SalesReturnDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetAllSalesReturnsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<SalesReturnDto>>> Handle(GetAllSalesReturnsQuery query, CancellationToken cancellationToken)
        {
            var salesReturnsQuery = _context.SalesReturns
                .Include(sr => sr.SalesReturnItems)
                .Where(sr => !sr.IsDeleted);

            // Filter by sales if provided
            if (query.SalesId.HasValue)
            {
                salesReturnsQuery = salesReturnsQuery.Where(sr => sr.SalesId == query.SalesId.Value);
            }

            // Search by return number, sales number, or remark
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchLower = query.SearchTerm.ToLower();
                salesReturnsQuery = salesReturnsQuery.Where(sr =>
                    sr.ReturnNumber.ToLower().Contains(searchLower) ||
                    sr.Remark.ToLower().Contains(searchLower) ||
                    _context.Sales.Any(s => s.Id == sr.SalesId && s.SalesNumber.ToLower().Contains(searchLower))
                );
            }

            var salesReturnsDto = salesReturnsQuery
                .OrderByDescending(sr => sr.CreatedUtcDate)
                .Select(sr => new SalesReturnDto
                {
                    Id = sr.Id,
                    ReturnNumber = sr.ReturnNumber,
                    SalesId = sr.SalesId,
                    SalesNumber = _context.Sales.Where(s => s.Id == sr.SalesId).Select(s => s.SalesNumber).FirstOrDefault() ?? "",
                    TotalRefundAmount = sr.TotalRefundAmount,
                    Remark = sr.Remark,
                    CreatedUtcDate = sr.CreatedUtcDate.DateTime,
                    SalesReturnItems = sr.SalesReturnItems.Select(item => new SalesReturnItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductTitle = _context.Products.Where(p => p.Id == item.ProductId).Select(p => p.Title).FirstOrDefault() ?? "",
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice,
                        Reason = item.Reason
                    }).ToList()
                });

            var pagination = await new Pagination<SalesReturnDto>().CreateAsync(salesReturnsDto, query.PageNumber, query.PageSize, cancellationToken);

            return Response.Success(pagination);
        }
    }
}
