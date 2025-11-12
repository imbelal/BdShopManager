using Common.RequestWrapper;
using Common.ResponseWrapper;
using Common.Services.Interfaces;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tenant.Queries
{
    public class GetTenantByIdQueryHandler : IQueryHandler<GetTenantByIdQuery, Domain.Entities.Tenant>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetTenantByIdQueryHandler(IReadOnlyApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IResponse<Domain.Entities.Tenant>> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
        {
            Guid tenantId = Guid.Parse(_currentUserService?.GetUser()?.Claims?.FirstOrDefault(c => c.Type == "tenantId")?.Value);

            var tenant = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);
            if (tenant == null)
                return Response.Fail<Domain.Entities.Tenant>("No Tenant found!!");

            return Response.Success(tenant);
        }
    }
}