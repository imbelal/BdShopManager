using Common.Events;

namespace Domain.Events
{
    public class SalesUpdatedEvent : IDomainEvent
    {
        public Guid SalesId { get; private set; }
        public List<SalesItemInfo> OldSalesItems { get; private set; }
        public List<SalesItemInfo> NewSalesItems { get; private set; }

        public SalesUpdatedEvent(Guid salesId, List<SalesItemInfo> oldSalesItems, List<SalesItemInfo> newSalesItems)
        {
            SalesId = salesId;
            OldSalesItems = oldSalesItems;
            NewSalesItems = newSalesItems;
        }
    }
}
