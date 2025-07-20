using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.User.Commands
{
    public class UpdateUserInfoCommandHandler : ICommandHandler<UpdateUserInfoCommand, Guid>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserInfoCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IResponse<Guid>> Handle(UpdateUserInfoCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null) throw new KeyNotFoundException("User not found!!");

            user.UpdateProfileInfo(command.Email, command.Firstname, command.Lastname);
            _userRepository.Update(user);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(user.Id);
        }
    }
}
