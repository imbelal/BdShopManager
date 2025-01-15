using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.UserRole.Commands
{
    public class CreateUserRoleCommandHandler : ICommandHandler<CreateUserRoleCommand, Guid>
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public CreateUserRoleCommandHandler(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public async Task<IResponse<Guid>> Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var userRole = new Domain.Entities.UserRole(request.Title);

            _userRoleRepository.Add(userRole);
            await _userRoleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(userRole.Id);
        }
    }
}
