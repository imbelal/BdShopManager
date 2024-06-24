using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.User.Queries
{
    public class GetUserByIdQuery : IQuery<UserDto>
    {
        public Guid Id { get; set; }

        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
