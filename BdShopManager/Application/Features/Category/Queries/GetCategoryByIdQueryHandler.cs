using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Category.Queries
{
    public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, Domain.Entities.Category>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetCategoryByIdQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Domain.Entities.Category>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (category == null)
                return Response.Fail<Domain.Entities.Category>("No Category found!!");

            return Response.Success(category);
        }
    }
}
