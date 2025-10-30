using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Order.Queries
{
    public class GetAllOrdersQuery : IQuery<Pagination<OrderDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? CustomerId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
