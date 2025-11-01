using Common.Events;

namespace Domain.Events
{
    public class SalesCancelledEvent : IDomainEvent
    {
        public Guid SalesId { get; private set; }
        public List<SalesItemInfo> SalesItems { get; private set; }

        public SalesCancelledEvent(Guid salesId, List<SalesItemInfo> salesItems)
        {
            SalesId = salesId;
            SalesItems = salesItems;
        }
    }
}