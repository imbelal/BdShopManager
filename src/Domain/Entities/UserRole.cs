
using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Enums;

namespace Domain.Entities
{
    public class UserRole : AuditableEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        public UserRoleType Type { get; private set; }

        public bool IsDeleted { get; set; } = false;

        public UserRole() : base()
        {

        }

        public UserRole(UserRoleType type) : base(Guid.NewGuid())
        {
            Type = type;
        }

        public void Update(UserRoleType type)
        {
            Type = type;
        }
    }
}
