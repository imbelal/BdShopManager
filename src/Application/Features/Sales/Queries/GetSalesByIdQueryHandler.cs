using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sales.Queries
{
    public class GetSalesByIdQueryHandler : IQueryHandler<GetSalesByIdQuery, SalesDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetSalesByIdQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<SalesDto>> Handle(GetSalesByIdQuery query, CancellationToken cancellationToken)
        {
            var sales = await _context.Sales
                .Include(o => o.SalesItems)
                .Where(o => o.Id == query.Id && !o.IsDeleted)
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
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (sales == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Sales not found!");
            }

            return Response.Success(sales);
        }
    }
}
