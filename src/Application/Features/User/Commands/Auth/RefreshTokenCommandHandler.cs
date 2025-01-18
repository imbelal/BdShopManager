using Application.Services.Exceptions;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos.Auth;
using Domain.Interfaces;
using Domain.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Commands.Auth
{
    public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthenticateResponse>
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly IRefreshTokenValidator _refreshTokenValidator;
        private readonly IReadOnlyApplicationDbContext _context;
        private readonly IUserRepository _userRepository;

        public RefreshTokenCommandHandler(IRefreshTokenValidator refreshTokenValidator, IReadOnlyApplicationDbContext context, IAuthenticateService authenticateService, IUserRepository userRepository)
        {
            _refreshTokenValidator = refreshTokenValidator;
            _context = context;
            _authenticateService = authenticateService;
            _userRepository = userRepository;
        }

        public async Task<IResponse<AuthenticateResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshRequest = request.RefreshTokenRequest;
            var isValidRefreshToken = _refreshTokenValidator.Validate(refreshRequest.RefreshToken);
            if (!isValidRefreshToken)
                throw new InvalidRefreshTokenException();

            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshRequest.RefreshToken, cancellationToken);
            if (refreshToken is null)
                throw new InvalidRefreshTokenException();

            _userRepository.RemoveRefreshToken(refreshToken);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            var user = await _context.Users.Include(u => u.UserRole).FirstOrDefaultAsync(x => x.Id == refreshToken.UserId);
            if (user is null) throw new UserNotFoundException();

            return Response.Success(await _authenticateService.Authenticate(user, cancellationToken));
        }
    }
}
