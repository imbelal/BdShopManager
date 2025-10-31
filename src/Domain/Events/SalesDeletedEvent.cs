using Common.Events;

namespace Domain.Events
{
    public class SalesDeletedEvent : IDomainEvent
    {
        public Guid SalesId { get; private set; }
        public List<SalesItemInfo> SalesItems { get; private set; }

        public SalesDeletedEvent(Guid salesId, List<SalesItemInfo> salesItems)
        {
            SalesId = salesId;
            SalesItems = salesItems;
        }
    }
}
