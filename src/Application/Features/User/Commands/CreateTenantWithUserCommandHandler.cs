using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Features.User.Commands
{
    public class CreateTenantWithUserCommandHandler : ICommandHandler<CreateTenantWithUserCommand, Guid>
    {
        private readonly IReadOnlyApplicationDbContext _applicationDbContext;
        private readonly ITenantRepository _tenantRepository;

        public CreateTenantWithUserCommandHandler(IReadOnlyApplicationDbContext applicationDbContext, ITenantRepository tenantRepository)
        {
            _applicationDbContext = applicationDbContext;
            _tenantRepository = tenantRepository;
        }

        public async Task<IResponse<Guid>> Handle(CreateTenantWithUserCommand request, CancellationToken cancellationToken)
        {
            Tenant tenant = new(request.UserToCreate);
            _tenantRepository.Add(tenant);
            await _tenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(tenant.Id);
        }
    }
}
