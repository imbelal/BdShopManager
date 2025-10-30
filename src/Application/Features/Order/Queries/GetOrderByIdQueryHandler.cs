using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Queries
{
    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetOrderByIdQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<OrderDto>> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Id == query.Id && !o.IsDeleted)
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
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                throw new Exception("Order not found!");
            }

            return Response.Success(order);
        }
    }
}
