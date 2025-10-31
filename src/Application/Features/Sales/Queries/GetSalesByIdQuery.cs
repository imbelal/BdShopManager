using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Sales.Queries
{
    public class GetSalesByIdQuery : IQuery<SalesDto>
    {
        public Guid Id { get; set; }
    }
}
