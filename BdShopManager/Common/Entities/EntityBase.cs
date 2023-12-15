using Common.Entities.Interfaces;
using Common.Events;

namespace Common.Entities
{
    public abstract class EntityBase : IEntityBase
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public Guid Id { get; set; }

        public IReadOnlyList<IDomainEvent> GetDomainEvents()
        {
            return _domainEvents.ToList();
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
