using Common.RequestWrapper;

namespace Application.Features.UserRole.Queires
{
    public class GetUserRoleByIdQuery : IQuery<Domain.Entities.UserRole>
    {
        public Guid Id { get; set; }

        public GetUserRoleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
