using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sales.Queries
{
    public class GetAllSalesQueryHandler : IQueryHandler<GetAllSalesQuery, Pagination<SalesDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetAllSalesQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<SalesDto>>> Handle(GetAllSalesQuery query, CancellationToken cancellationToken)
        {
            var salesQuery = _context.Sales
                .Include(o => o.SalesItems)
                .Where(o => !o.IsDeleted);

            // Filter by customer if provided
            if (query.CustomerId.HasValue)
            {
                salesQuery = salesQuery.Where(o => o.CustomerId == query.CustomerId.Value);
            }

            // Search by customer name or remark
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchLower = query.SearchTerm.ToLower();
                salesQuery = salesQuery.Where(o =>
                    o.Remark.ToLower().Contains(searchLower) ||
                    _context.Customers.Any(c => c.Id == o.CustomerId &&
                        (c.FirstName.ToLower().Contains(searchLower) || c.LastName.ToLower().Contains(searchLower)))
                );
            }

            var salesDto = salesQuery
                .OrderByDescending(o => o.CreatedUtcDate)
                .Select(o => new SalesDto
                {
                    Id = o.Id,
                    SalesNumber = o.SalesNumber,
                    CustomerId = o.CustomerId,
                    CustomerName = _context.Customers.Where(c => c.Id == o.CustomerId).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault() ?? "",
                    TotalPrice = o.TotalPrice,
                    DiscountPercentage = o.DiscountPercentage,
                    DiscountAmount = o.DiscountAmount,
                    DiscountedPrice = o.DiscountedPrice,
                    TaxPercentage = o.TaxPercentage,
                    TaxAmount = o.TaxAmount,
                    GrandTotal = o.GrandTotal,
                    TotalPaid = o.TotalPaid,
                    RemainingAmount = o.GrandTotal - o.TotalPaid,
                    Status = o.Status,
                    Remark = o.Remark,
                    CreatedBy = o.CreatedBy,
                    CreatedDate = o.CreatedUtcDate.DateTime,
                    SalesItems = o.SalesItems.Select(od => new SalesItemDto
                    {
                        Id = od.Id,
                        ProductId = od.ProductId,
                        ProductName = _context.Products.Where(p => p.Id == od.ProductId).Select(p => p.Title).FirstOrDefault() ?? "",
                        Quantity = od.Quantity,
                        Unit = od.Unit,
                        UnitPrice = od.UnitPrice,
                        TotalPrice = od.TotalPrice
                    }).ToList()
                });

            var pagination = await new Pagination<SalesDto>().CreateAsync(salesDto, query.PageNumber, query.PageSize, cancellationToken);

            return Response.Success(pagination);
        }
    }
}
