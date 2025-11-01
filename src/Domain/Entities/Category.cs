using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Events;

namespace Domain.Entities
{
    public class Category : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        public string Title { get; set; }
        public bool IsDeleted { get; set; }
        public Tenant Tenant { get; set; }

        public Category() : base()
        {

        }

        public Category(string title) : base(Guid.NewGuid())
        {
            Title = title;
        }

        public void RaiseCreatedEvent()
        {
            RaiseDomainEvent(new CategoryCreatedEvent());
        }

        public void RaiseUpdatedEvent()
        {
            RaiseDomainEvent(new CategoryUpdatedEvent());
        }
    }
}
