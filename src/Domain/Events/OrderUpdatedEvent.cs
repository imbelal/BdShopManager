using Common.Events;

namespace Domain.Events
{
    public class OrderUpdatedEvent : IDomainEvent
    {
        public Guid OrderId { get; private set; }
        public List<OrderDetailInfo> OldOrderDetails { get; private set; }
        public List<OrderDetailInfo> NewOrderDetails { get; private set; }

        public OrderUpdatedEvent(Guid orderId, List<OrderDetailInfo> oldOrderDetails, List<OrderDetailInfo> newOrderDetails)
        {
            OrderId = orderId;
            OldOrderDetails = oldOrderDetails;
            NewOrderDetails = newOrderDetails;
        }
    }
}
