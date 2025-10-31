using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Sales.Queries
{
    public class GetAllSalesQuery : IQuery<Pagination<SalesDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? CustomerId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
