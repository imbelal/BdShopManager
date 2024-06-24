using Common.RequestWrapper;

namespace Application.Features.Product.Queries
{
    public class GetProductByIdQuery : IQuery<Domain.Entities.Product>
    {
        public Guid Id { get; private set; }

        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
