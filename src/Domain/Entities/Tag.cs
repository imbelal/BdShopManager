using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class Tag : AuditableTenantEntityBase<Guid>, IAggregateRoot
    {
        public string Title { get; private set; }

        public Tag() : base()
        {

        }

        public Tag(string title) : base(Guid.NewGuid())
        {
            Title = title;
        }

        public void Update(string title)
        {
            Title = title;
        }
    }
}
