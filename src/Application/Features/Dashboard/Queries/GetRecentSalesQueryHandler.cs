using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;

namespace Application.Features.Dashboard.Queries
{
    public class GetRecentSalesQueryHandler : IQueryHandler<GetRecentSalesQuery, Pagination<RecentSaleDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetRecentSalesQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<RecentSaleDto>>> Handle(GetRecentSalesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var salesQuery = _context.Sales
                    .Where(s => !s.IsDeleted)
                    .Join(_context.Customers.Where(c => !c.IsDeleted),
                          s => s.CustomerId,
                          c => c.Id,
                          (s, c) => new { Sales = s, Customer = c })
                    .OrderByDescending(x => x.Sales.CreatedUtcDate)
                    .Select(x => new RecentSaleDto
                    {
                        Id = x.Sales.Id.ToString(),
                        SaleDate = x.Sales.CreatedUtcDate.UtcDateTime,
                        CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                        CustomerPhone = x.Customer.ContactNo,
                        TotalItems = x.Sales.SalesItems.Count(),
                        TotalAmount = x.Sales.TotalPrice,
                        PaymentStatus = x.Sales.Status.ToString(),
                        PaymentMethod = x.Sales.Payments.FirstOrDefault().PaymentMethod,
                        Status = x.Sales.Status.ToString(),
                        CreatedBy = x.Sales.CreatedBy,
                        FormattedSaleDate = x.Sales.CreatedUtcDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        FormattedTotalAmount = x.Sales.TotalPrice.ToString("N2")
                    });

                var pagination = await new Pagination<RecentSaleDto>()
                    .CreateAsync(salesQuery, request.PageNumber, request.PageSize, cancellationToken);

                if (pagination.Items.Count == 0)
                    return Response.Fail<Pagination<RecentSaleDto>>("No recent sales found");

                return Response.Success(pagination);
            }
            catch (Exception ex)
            {
                return Response.Fail<Pagination<RecentSaleDto>>($"Error retrieving recent sales: {ex.Message}");
            }
        }
    }
}