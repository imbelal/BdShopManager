using Common.Events;

namespace Domain.Events
{
    public class OrderDeletedEvent : IDomainEvent
    {
        public Guid OrderId { get; private set; }
        public List<OrderDetailInfo> OrderDetails { get; private set; }

        public OrderDeletedEvent(Guid orderId, List<OrderDetailInfo> orderDetails)
        {
            OrderId = orderId;
            OrderDetails = orderDetails;
        }
    }
}
