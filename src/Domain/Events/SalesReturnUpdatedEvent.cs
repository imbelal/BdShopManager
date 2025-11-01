using Common.Events;

namespace Domain.Events
{
    public class SalesReturnUpdatedEvent : IDomainEvent
    {
        public Guid SalesReturnId { get; private set; }
        public List<SalesReturnItemInfo> OldReturnItems { get; private set; }
        public List<SalesReturnItemInfo> NewReturnItems { get; private set; }

        public SalesReturnUpdatedEvent(Guid salesReturnId, List<SalesReturnItemInfo> oldReturnItems, List<SalesReturnItemInfo> newReturnItems)
        {
            SalesReturnId = salesReturnId;
            OldReturnItems = oldReturnItems;
            NewReturnItems = newReturnItems;
        }
    }
}