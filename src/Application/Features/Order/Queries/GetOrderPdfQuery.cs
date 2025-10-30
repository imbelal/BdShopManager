using Common.RequestWrapper;

namespace Application.Features.Order.Queries
{
    public class GetOrderPdfQuery : IQuery<byte[]>
    {
        public Guid OrderId { get; set; }
    }
}
