using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.UserRole.Commands
{
    public class DeleteUserRoleCommandHandler : ICommandHandler<DeleteUserRoleCommand, Guid>
    {
        private readonly IUserRoleRepository _userRoleRepository;
        public DeleteUserRoleCommandHandler(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }
        public async Task<IResponse<Guid>> Handle(DeleteUserRoleCommand request, CancellationToken cancellationToken)
        {
            var userRole = await _userRoleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (userRole == null) throw new KeyNotFoundException();

            _userRoleRepository.Remove(userRole);
            await _userRoleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(userRole.Id);
        }
    }
}
