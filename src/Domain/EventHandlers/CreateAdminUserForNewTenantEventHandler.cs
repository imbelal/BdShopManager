using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Domain.Interfaces.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.EventHandlers
{
    public class CreateAdminUserForNewTenantEventHandler : INotificationHandler<TenantCreatedEvent>
    {
        private readonly IReadOnlyApplicationDbContext _applicationDbContext;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IPasswordHasher _passwordHasher;

        public CreateAdminUserForNewTenantEventHandler(
            IReadOnlyApplicationDbContext readOnlyApplicationDbContext,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IPasswordHasher passwordHasher)
        {
            _applicationDbContext = readOnlyApplicationDbContext;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(TenantCreatedEvent @event, CancellationToken cancellationToken)
        {

            var tenantId = await _applicationDbContext.Tenants
                                                      .Where(tenant => tenant.Id.Equals(@event.TenantId))
                                                      .Select(tenant => (Guid?)tenant.Id)
                                                      .FirstOrDefaultAsync(cancellationToken) ?? throw new KeyNotFoundException("Given tenant is not found");

            UserRole userRole = new(Enums.UserRoleType.Admin);

            var passwordHash = _passwordHasher.CreateHash(@event.Password);
            Domain.Entities.User newUser = new(
                    @event.Username,
                    passwordHash,
                    @event.Email,
                    @event.FirstName,
                    @event.LastName,
                    userRole.Id,
                    tenantId);

            _userRoleRepository.Add(userRole);
            _userRepository.Add(newUser);
        }
    }
}
