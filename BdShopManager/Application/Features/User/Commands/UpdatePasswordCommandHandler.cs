using Application.Services.Auth.Interfaces;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.User.Commands
{
    public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UpdatePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<IResponse<Guid>> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId) ?? throw new KeyNotFoundException("User not found!!");
            if (user.Username != command.Username) throw new KeyNotFoundException("Username or email is not correct!!");

            (bool isVarified, bool needUpgrade) = _passwordHasher.VerifyPassword(user.PasswordHash, command.OldPassword);
            if (isVarified == false)
                throw new InvalidDataException("Old password is not correct!!");

            var newPasswordHash = _passwordHasher.CreateHash(command.NewPassword);
            user.ChangePassword(user.PasswordHash, newPasswordHash);

            _userRepository.Update(user);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(user.Id);
        }
    }
}
