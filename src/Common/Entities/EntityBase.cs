using Common.Entities.Interfaces;
using Common.Events;

namespace Common.Entities
{
    public abstract class EntityBase<TIdentity> : IEntityBase
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public TIdentity Id { get; private set; }

        public IReadOnlyList<IDomainEvent> GetDomainEvents()
        {
            return _domainEvents.ToList();
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        protected EntityBase()
        {

        }

        protected EntityBase(TIdentity id)
        {
            Id = id;
        }
    }
}
