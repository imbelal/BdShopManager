using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Order.Queries
{
    public class GetPaymentsByOrderQuery : IQuery<List<PaymentDto>>
    {
        public Guid OrderId { get; set; }
    }
}
