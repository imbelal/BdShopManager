using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Order.Queries
{
    public class GetOrderByIdQuery : IQuery<OrderDto>
    {
        public Guid Id { get; set; }
    }
}
