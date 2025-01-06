using Application.Services.Auth.Interfaces;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Commands
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
    {
        private readonly IReadOnlyApplicationDbContext _applicationDbContext;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(IReadOnlyApplicationDbContext applicationDbContext, IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _applicationDbContext = applicationDbContext;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<IResponse<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var userRoleId = await _applicationDbContext.UserRoles
                                                      .Where(userRole => userRole.Id.Equals(command.userToCreate.UserRoleId))
                                                      .Select(userRole => (Guid?)userRole.Id)
                                                      .FirstOrDefaultAsync(cancellationToken) ?? throw new KeyNotFoundException("Given user role is not found");
            var tenantId = await _applicationDbContext.Tenants
                                                      .Where(tenant => tenant.Id.Equals(command.userToCreate.TenantId))
                                                      .Select(tenant => (Guid?)tenant.Id)
                                                      .FirstOrDefaultAsync(cancellationToken) ?? throw new KeyNotFoundException("Given tenant is not found");

            var passwordHash = _passwordHasher.CreateHash(command.userToCreate.Password);
            Domain.Entities.User newUser = new(
                    command.userToCreate.Username,
                    passwordHash,
                    command.userToCreate.Email,
                    command.userToCreate.FirstName,
                    command.userToCreate.LastName,
                    userRoleId,
                    tenantId);

            _userRepository.Add(newUser);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(newUser.Id);
        }
    }
}
