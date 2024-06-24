using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserRole.Queires
{
    public class GetAllUserRolesQueryHandler : IQueryHandler<GetAllUserRolesQuery, List<Domain.Entities.UserRole>>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetAllUserRolesQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IResponse<List<Domain.Entities.UserRole>>> Handle(GetAllUserRolesQuery query, CancellationToken cancellationToken)
        {
            var userRoles = await _context.UserRoles.ToListAsync(cancellationToken);
            if (userRoles.Count == 0)
                return Response.Fail<List<Domain.Entities.UserRole>>("No User role found!!");

            return Response.Success(userRoles);
        }
    }
}
