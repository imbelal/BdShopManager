using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Queries
{
    public class GetPaymentsByOrderQueryHandler : IQueryHandler<GetPaymentsByOrderQuery, List<PaymentDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetPaymentsByOrderQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<PaymentDto>>> Handle(GetPaymentsByOrderQuery query, CancellationToken cancellationToken)
        {
            var payments = await _context.Payments
                .Where(p => p.OrderId == query.OrderId)
                .OrderByDescending(p => p.CreatedUtcDate)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
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
