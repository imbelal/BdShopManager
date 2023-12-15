using Common.RequestWrapper;

namespace Application.Features.Category.Queries
{
    public class GetCategoryByIdQuery : IQuery<Domain.Entities.Category>
    {
        public Guid Id { get; private set; }

        public GetCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
