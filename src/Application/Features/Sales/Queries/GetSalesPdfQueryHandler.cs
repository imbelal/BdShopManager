using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sales.Queries
{
    public class GetSalesPdfQueryHandler : IQueryHandler<GetSalesPdfQuery, byte[]>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        private readonly IPdfGeneratorService _pdfGeneratorService;

        public GetSalesPdfQueryHandler(IReadOnlyApplicationDbContext context, IPdfGeneratorService pdfGeneratorService)
        {
            _context = context;
            _pdfGeneratorService = pdfGeneratorService;
        }

        public async Task<IResponse<byte[]>> Handle(GetSalesPdfQuery query, CancellationToken cancellationToken)
        {
            // Get the sales with all details
            var sales = await _context.Sales
                .Include(o => o.SalesItems)
                .Where(o => o.Id == query.SalesId && !o.IsDeleted)
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

            // Get tenant details
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == sales.CustomerId, cancellationToken);
            if (customer == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Customer not found!");
            }

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == customer.TenantId, cancellationToken);
            if (tenant == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Tenant not found!");
            }

            // Generate PDF
            var pdfBytes = _pdfGeneratorService.GenerateSalesPdf(
                sales,
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
