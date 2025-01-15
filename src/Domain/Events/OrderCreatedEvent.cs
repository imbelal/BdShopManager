using Common.Events;

namespace Domain.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public Guid OrderId { get; private set; }

        public OrderCreatedEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
