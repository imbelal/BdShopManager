using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Customer.Queries
{
    public class GetAllCustomersWithPaginationQuery : IQuery<Pagination<CustomerDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }
}