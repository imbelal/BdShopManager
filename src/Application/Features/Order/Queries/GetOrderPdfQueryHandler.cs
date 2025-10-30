using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Queries
{
    public class GetOrderPdfQueryHandler : IQueryHandler<GetOrderPdfQuery, byte[]>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        private readonly IPdfGeneratorService _pdfGeneratorService;

        public GetOrderPdfQueryHandler(IReadOnlyApplicationDbContext context, IPdfGeneratorService pdfGeneratorService)
        {
            _context = context;
            _pdfGeneratorService = pdfGeneratorService;
        }

        public async Task<IResponse<byte[]>> Handle(GetOrderPdfQuery query, CancellationToken cancellationToken)
        {
            // Get the order with all details
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Id == query.OrderId && !o.IsDeleted)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerId = o.CustomerId,
                    CustomerName = _context.Customers.Where(c => c.Id == o.CustomerId).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault() ?? "",
                    TotalPrice = o.TotalPrice,
                    TaxPercentage = o.TaxPercentage,
                    TaxAmount = o.TaxAmount,
                    GrandTotal = o.GrandTotal,
                    TotalPaid = o.TotalPaid,
                    RemainingAmount = o.GrandTotal - o.TotalPaid,
                    Status = o.Status,
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

            // Get tenant details
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == order.CustomerId, cancellationToken);
            if (customer == null)
            {
                throw new Exception("Customer not found!");
            }

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == customer.TenantId, cancellationToken);
            if (tenant == null)
            {
                throw new Exception("Tenant not found!");
            }

            // Generate PDF
            var pdfBytes = _pdfGeneratorService.GenerateOrderPdf(
                order,
                tenant.Name,
                tenant.Address,
                tenant.PhoneNumber,
                customer.Address,
                customer.ContactNo
            );

            return Response.Success(pdfBytes);
        }
    }
}
