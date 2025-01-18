using Application.Services.Exceptions;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos.Auth;
using Domain.Interfaces;
using Domain.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Commands.Auth
{
    public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthenticateResponse>
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IReadOnlyApplicationDbContext _context;

        public LoginUserCommandHandler(IPasswordHasher passwordHasher, IAuthenticateService authenticateService, IReadOnlyApplicationDbContext context)
        {
            _passwordHasher = passwordHasher;
            _authenticateService = authenticateService;
            _context = context;
        }

        public async Task<IResponse<AuthenticateResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(x => x.Username == command.LoginUserRequest.Username, cancellationToken);
            if (user == null) throw UserNotFoundException.Instance;
            (bool varified, bool needToUpgrade) = _passwordHasher.VerifyPassword(user.PasswordHash, command.LoginUserRequest.Password);

            if (varified == false) throw SignInException.Instance;
            return Response.Success(await _authenticateService.Authenticate(user, cancellationToken));
        }
    }
}
