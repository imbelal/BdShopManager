using Common.RequestWrapper;
using Common.ResponseWrapper;
using Common.Services.Interfaces;
using Domain.Interfaces;
using Domain.Entities;

namespace Application.Features.Tenant.Commands
{
    public class UpdateTenantCommandHandler : ICommandHandler<UpdateTenantCommand, Guid>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateTenantCommandHandler(ITenantRepository tenantRepository, ICurrentUserService currentUserService)
        {
            _tenantRepository = tenantRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IResponse<Guid>> Handle(UpdateTenantCommand command, CancellationToken cancellationToken)
        {
            Guid tenantId = Guid.Parse(_currentUserService?.GetUser()?.Claims?.FirstOrDefault(c => c.Type == "tenantId")?.Value);

            Domain.Entities.Tenant tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
            if (tenant == null) throw new Common.Exceptions.BusinessLogicException("Tenant not found!!");

            tenant.Name = command.Name;
            tenant.Address = command.Address;
            tenant.PhoneNumber = command.PhoneNumber;

            _tenantRepository.Update(tenant);
            await _tenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(tenant.Id);
        }
    }
}