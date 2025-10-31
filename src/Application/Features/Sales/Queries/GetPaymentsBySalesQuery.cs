using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Sales.Queries
{
    public class GetPaymentsBySalesQuery : IQuery<List<PaymentDto>>
    {
        public Guid SalesId { get; set; }
    }
}
