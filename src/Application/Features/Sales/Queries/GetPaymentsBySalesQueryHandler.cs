using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sales.Queries
{
    public class GetPaymentsBySalesQueryHandler : IQueryHandler<GetPaymentsBySalesQuery, List<PaymentDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetPaymentsBySalesQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<PaymentDto>>> Handle(GetPaymentsBySalesQuery query, CancellationToken cancellationToken)
        {
            var payments = await _context.Payments
                .Where(p => p.SalesId == query.SalesId)
                .OrderByDescending(p => p.CreatedUtcDate)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    SalesId = p.SalesId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Remark = p.Remark,
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedUtcDate.DateTime
                })
                .ToListAsync(cancellationToken);

            return Response.Success(payments);
        }
    }
}
