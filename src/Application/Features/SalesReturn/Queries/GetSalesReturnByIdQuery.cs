using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.SalesReturn.Queries
{
    public class GetSalesReturnByIdQuery : IQuery<SalesReturnDto>
    {
        public Guid Id { get; set; }
    }
}
