using Common.Events;

namespace Common.Entities.Interfaces
{
    public interface IEntityBase
    {
        IReadOnlyList<IDomainEvent> GetDomainEvents();

        void ClearDomainEvents();

        void RaiseDomainEvent(IDomainEvent domainEvent);
    }
}
