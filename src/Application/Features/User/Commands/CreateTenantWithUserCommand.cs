using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.User.Commands
{
    public class CreateTenantWithUserCommand : ICommand<Guid>
    {
        public CreateTenantWithUserRequestDto UserToCreate { get; set; }

        public CreateTenantWithUserCommand(CreateTenantWithUserRequestDto userToCreate)
        {
            UserToCreate = userToCreate;
        }
    }
}
