using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Dtos;
using Domain.Enums;
using Domain.Events;

namespace Domain.Entities
{
    public class Order : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        private List<OrderDetail> orderDetails = new();
        private List<Payment> payments = new();
        public string OrderNumber { get; private set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TaxPercentage { get; set; } = 0;
        public decimal TotalPaid { get; private set; }
        public OrderStatus Status { get; private set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        // Calculated properties
        public decimal TaxAmount => TotalPrice * (TaxPercentage / 100);
        public decimal GrandTotal => TotalPrice + TaxAmount;

        // Profit calculation properties
        public decimal OrderTotalCost => orderDetails.Sum(od => od.TotalCost);
        public decimal OrderProfit => TotalPrice - OrderTotalCost;
        public decimal OrderProfitMargin => TotalPrice > 0
            ? Math.Round((OrderProfit / TotalPrice) * 100, 2)
            : 0;

        public IReadOnlyCollection<OrderDetail> OrderDetails
        {
            get => orderDetails;
        }

        public IReadOnlyCollection<Payment> Payments
        {
            get => payments;
        }

        public Order() : base()
        {

        }

        public Order(Guid customerId, decimal totalPrice, decimal totalPaid, string remark, decimal taxPercentage = 0) : base(Guid.NewGuid())
        {
            CustomerId = customerId;
            TotalPrice = totalPrice;
            TaxPercentage = taxPercentage;
            TotalPaid = totalPaid;
            Remark = remark;

            // Set initial status based on payment
            UpdatePaymentStatus();
        }

        public static Order CreateOrderWithDetails(Guid customerId, decimal totalPrice, decimal totalPaid, string remark, List<OrderDetailsDto> orderDetailDtos, decimal taxPercentage = 0)
        {
            // Validate total price matches order details
            decimal calculatedTotal = orderDetailDtos.Sum(od => od.Quantity * od.UnitPrice);
            if (Math.Abs(calculatedTotal - totalPrice) > 0.01m) // Allow for minor rounding differences
            {
                throw new Common.Exceptions.BusinessLogicException($"Total price mismatch. Provided: {totalPrice:N2}, Calculated from order details: {calculatedTotal:N2}");
            }

            Order newOrder = new(customerId, totalPrice, totalPaid, remark, taxPercentage);
            newOrder.orderDetails.AddRange(
                orderDetailDtos.Select(orderDetail => new OrderDetail(newOrder.Id,
                orderDetail.ProductId,
                orderDetail.Quantity,
                orderDetail.Unit,
                orderDetail.UnitPrice,
                orderDetail.UnitCost)).ToList()
            );

            // Raise domain event with order details for stock management
            var orderDetailInfos = orderDetailDtos.Select(od => new OrderDetailInfo(od.ProductId, od.Quantity)).ToList();
            newOrder.RaiseDomainEvent(new OrderCreatedEvent(newOrder.Id, orderDetailInfos));

            return newOrder;
        }

        public void Update(Guid customerId, decimal totalPrice, decimal totalPaid, string remark, List<OrderDetailsDto> orderDetailDtos, decimal taxPercentage = 0)
        {
            // Validate total price matches order details
            decimal calculatedTotal = orderDetailDtos.Sum(od => od.Quantity * od.UnitPrice);
            if (Math.Abs(calculatedTotal - totalPrice) > 0.01m) // Allow for minor rounding differences
            {
                throw new Common.Exceptions.BusinessLogicException($"Total price mismatch. Provided: {totalPrice:N2}, Calculated from order details: {calculatedTotal:N2}");
            }

            // Capture old order details before clearing for stock adjustment
            var oldOrderDetailInfos = orderDetails.Select(od => new OrderDetailInfo(od.ProductId, od.Quantity)).ToList();

            CustomerId = customerId;
            TotalPrice = totalPrice;
            TaxPercentage = taxPercentage;
            TotalPaid = totalPaid;
            Remark = remark;

            // Clear existing order details and add new ones
            orderDetails.Clear();
            orderDetails.AddRange(
                orderDetailDtos.Select(orderDetail => new OrderDetail(this.Id,
                orderDetail.ProductId,
                orderDetail.Quantity,
                orderDetail.Unit,
                orderDetail.UnitPrice,
                orderDetail.UnitCost)).ToList()
            );

            // Raise domain event for stock adjustment
            var newOrderDetailInfos = orderDetailDtos.Select(od => new OrderDetailInfo(od.ProductId, od.Quantity)).ToList();
            RaiseDomainEvent(new OrderUpdatedEvent(this.Id, oldOrderDetailInfos, newOrderDetailInfos));

            // Update payment status
            UpdatePaymentStatus();
        }

        public void Delete()
        {
            IsDeleted = true;

            // Raise domain event to restore stock for deleted order
            var orderDetailInfos = orderDetails.Select(od => new OrderDetailInfo(od.ProductId, od.Quantity)).ToList();
            RaiseDomainEvent(new OrderDeletedEvent(this.Id, orderDetailInfos));
        }

        public void SetOrderNumber(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                throw new Common.Exceptions.BusinessLogicException("Order number cannot be empty.");
            }
            OrderNumber = orderNumber;
        }

        public static string GenerateOrderNumber(DateTime date, int sequenceNumber)
        {
            var datePrefix = date.ToString("yyyyMMdd");
            return $"ORD-{datePrefix}-{sequenceNumber:D3}";
        }

        public void AddPayment(decimal amount, string paymentMethod, string remark)
        {
            if (amount <= 0)
            {
                throw new Common.Exceptions.BusinessLogicException("Payment amount must be greater than zero.");
            }

            if (Status == OrderStatus.Paid)
            {
                throw new Common.Exceptions.BusinessLogicException("Order is already fully paid.");
            }

            if (Status == OrderStatus.Cancelled)
            {
                throw new Common.Exceptions.BusinessLogicException("Cannot add payment to a cancelled order.");
            }

            // Create and add new payment
            var payment = new Payment(this.Id, amount, paymentMethod, remark);
            payments.Add(payment);

            // Update total paid
            TotalPaid += amount;

            // Update order status
            UpdatePaymentStatus();
        }

        private void UpdatePaymentStatus()
        {
            if (TotalPaid == 0)
            {
                Status = OrderStatus.Pending;
            }
            else if (TotalPaid >= GrandTotal)
            {
                Status = OrderStatus.Paid;
            }
            else
            {
                Status = OrderStatus.PartiallyPaid;
            }
        }
    }
}
