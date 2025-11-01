using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.SalesReturn.Queries
{
    public class GetAllSalesReturnsQuery : IQuery<Pagination<SalesReturnDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? SalesId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
