using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SalesReturn.Queries
{
    public class GetSalesReturnByIdQueryHandler : IQueryHandler<GetSalesReturnByIdQuery, SalesReturnDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetSalesReturnByIdQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<SalesReturnDto>> Handle(GetSalesReturnByIdQuery query, CancellationToken cancellationToken)
        {
            var salesReturn = await _context.SalesReturns
                .Include(sr => sr.SalesReturnItems)
                .Where(sr => sr.Id == query.Id && !sr.IsDeleted)
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
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (salesReturn == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Sales return not found!");
            }

            return Response.Success(salesReturn);
        }
    }
}
