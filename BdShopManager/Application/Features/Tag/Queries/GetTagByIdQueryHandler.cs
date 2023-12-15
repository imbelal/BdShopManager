using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tag.Queries
{
    public class GetTagByIdQueryHandler : IQueryHandler<GetTagByIdQuery, Domain.Entities.Tag>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetTagByIdQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Domain.Entities.Tag>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (tag == null)
                return Response.Fail<Domain.Entities.Tag>("No tag found!!");

            return Response.Success(tag);
        }
    }
}
