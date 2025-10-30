using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Queries
{
    public class GetAllOrdersQueryHandler : IQueryHandler<GetAllOrdersQuery, Pagination<OrderDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetAllOrdersQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<OrderDto>>> Handle(GetAllOrdersQuery query, CancellationToken cancellationToken)
        {
            var ordersQuery = _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => !o.IsDeleted);

            // Filter by customer if provided
            if (query.CustomerId.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.CustomerId == query.CustomerId.Value);
            }

            // Search by customer name or remark
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchLower = query.SearchTerm.ToLower();
                ordersQuery = ordersQuery.Where(o =>
                    o.Remark.ToLower().Contains(searchLower) ||
                    _context.Customers.Any(c => c.Id == o.CustomerId &&
                        (c.FirstName.ToLower().Contains(searchLower) || c.LastName.ToLower().Contains(searchLower)))
                );
            }

            var ordersDto = ordersQuery
                .OrderByDescending(o => o.CreatedUtcDate)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CustomerName = _context.Customers.Where(c => c.Id == o.CustomerId).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault() ?? "",
                    TotalPrice = o.TotalPrice,
                    TotalPaid = o.TotalPaid,
                    RemainingAmount = o.TotalPrice - o.TotalPaid,
                    Remark = o.Remark,
                    CreatedBy = o.CreatedBy,
                    CreatedDate = o.CreatedUtcDate.DateTime,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
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

            var pagination = await new Pagination<OrderDto>().CreateAsync(ordersDto, query.PageNumber, query.PageSize, cancellationToken);

            return Response.Success(pagination);
        }
    }
}
