using Common.RequestWrapper;

namespace Application.Features.Tag.Queries
{
    public class GetTagByIdQuery : IQuery<Domain.Entities.Tag>
    {
        public Guid Id { get; set; }

        public GetTagByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
