using Common.Events;

namespace Domain.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public Guid OrderId { get; private set; }
        public List<OrderDetailInfo> OrderDetails { get; private set; }

        public OrderCreatedEvent(Guid orderId, List<OrderDetailInfo> orderDetails)
        {
            OrderId = orderId;
            OrderDetails = orderDetails;
        }
    }

    public class OrderDetailInfo
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public OrderDetailInfo(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
