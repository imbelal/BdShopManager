using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.User.Commands
{
    public class CreateUserCommand : ICommand<Guid>
    {
        public CreateUserRequestDto userToCreate { get; set; }

        public CreateUserCommand(CreateUserRequestDto user)
        {
            userToCreate = user;
        }
    }
}
