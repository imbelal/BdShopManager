using Common.RequestWrapper;

namespace Application.Features.Sales.Queries
{
    public class GetSalesPdfQuery : IQuery<byte[]>
    {
        public Guid SalesId { get; set; }
    }
}
