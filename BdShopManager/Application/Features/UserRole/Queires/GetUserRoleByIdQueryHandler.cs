using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserRole.Queires
{
    public class GetUserRoleByIdQueryHandler : IQueryHandler<GetUserRoleByIdQuery, Domain.Entities.UserRole>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetUserRoleByIdQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IResponse<Domain.Entities.UserRole>> Handle(GetUserRoleByIdQuery query, CancellationToken cancellationToken)
        {
            var userRole = await _context.UserRoles.Where(a => a.Id == query.Id).FirstOrDefaultAsync();
            if (userRole == null)
                return Response.Fail<Domain.Entities.UserRole>("No user role found!!");

            return Response.Success(userRole);
        }
    }
}
