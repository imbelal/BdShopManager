using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.UserRole.Commands
{
    public class UpdateUserRoleCommandHandler : ICommandHandler<UpdateUserRoleCommand, Guid>
    {
        private readonly IUserRoleRepository _userRoleRepository;
        public UpdateUserRoleCommandHandler(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }
        public async Task<IResponse<Guid>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var userRole = await _userRoleRepository.GetByIdAsync(request.Id);
            if (userRole == null) throw new KeyNotFoundException();

            userRole.Update(request.Type);

            _userRoleRepository.Update(userRole);
            await _userRoleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(userRole.Id);
        }
    }
}
