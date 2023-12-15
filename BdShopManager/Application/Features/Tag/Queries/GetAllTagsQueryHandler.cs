using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tag.Queries
{
    public class GetAllTagsQueryHandler : IQueryHandler<GetAllTagsQuery, List<Domain.Entities.Tag>>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetAllTagsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<Domain.Entities.Tag>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _context.Tags.ToListAsync(cancellationToken);
            if (tags.Count == 0)
                return Response.Fail<List<Domain.Entities.Tag>>("No tag found!!");

            return Response.Success(tags);
        }
    }
}
