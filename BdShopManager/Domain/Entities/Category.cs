using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Events;

namespace Domain.Entities
{
    public class Category : AuditableEntityBase, IAggregateRoot, ISoftDeletable
    {
        public string Title { get; set; }
        public bool IsDeleted { get; set; }

        public Category()
        {

        }

        public Category(string title)
        {
            Title = title;
        }

        public void RaiseDeleteEvent()
        {
            RaiseDomainEvent(new CategoryDeletedEvent(this.Id));
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
