using Common.Events;

namespace Domain.Events
{
    public class SalesReturnDeletedEvent : IDomainEvent
    {
        public Guid SalesReturnId { get; private set; }
        public List<SalesReturnItemInfo> SalesReturnItems { get; private set; }

        public SalesReturnDeletedEvent(Guid salesReturnId, List<SalesReturnItemInfo> salesReturnItems)
        {
            SalesReturnId = salesReturnId;
            SalesReturnItems = salesReturnItems;
        }
    }
}
