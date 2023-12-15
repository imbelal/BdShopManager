using Application.Services.Auth.Interfaces;
using Application.Services.Common;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.User.Commands
{
    public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AppSettings _appSettings;

        public ResetPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _appSettings = appSettings.Value;
        }
        public async Task<IResponse<Guid>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId) ?? throw new KeyNotFoundException("User not found!!");

            var newPasswordHash = _passwordHasher.CreateHash(_appSettings.DefaultPassword);
            user.ChangePassword(user.PasswordHash, newPasswordHash);

            _userRepository.Update(user);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(user.Id);
        }
    }
}
