
using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class UserRole : AuditableEntityBase<Guid>, IAggregateRoot
    {
        public string Title { get; private set; }

        public UserRole() : base()
        {

        }

        public UserRole(string title) : base(Guid.NewGuid())
        {
            Title = title;
        }

        public void Update(string title)
        {
            Title = title;
        }
    }
}
