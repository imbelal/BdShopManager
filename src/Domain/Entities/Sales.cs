using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Dtos;
using Domain.Enums;
using Domain.Events;

namespace Domain.Entities
{
    public class Sales : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        private List<SalesItem> salesItems = new();
        private List<Payment> payments = new();
        public string SalesNumber { get; private set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountPercentage { get; set; } = 0;
        public decimal TaxPercentage { get; set; } = 0;
        public decimal TotalPaid { get; private set; }
        public SalesStatus Status { get; private set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        // Calculated properties
        public decimal DiscountAmount => Math.Round(TotalPrice * (DiscountPercentage / 100), 2);
        public decimal DiscountedPrice => TotalPrice - DiscountAmount;
        public decimal TaxAmount => DiscountedPrice * (TaxPercentage / 100);
        public decimal GrandTotal => DiscountedPrice + TaxAmount;

        // Profit calculation properties
        public decimal SalesTotalCost => salesItems.Sum(od => od.TotalCost);
        public decimal SalesProfit => DiscountedPrice - SalesTotalCost;
        public decimal SalesProfitMargin => DiscountedPrice > 0
            ? Math.Round((SalesProfit / DiscountedPrice) * 100, 2)
            : 0;

        public IReadOnlyCollection<SalesItem> SalesItems
        {
            get => salesItems;
        }

        public IReadOnlyCollection<Payment> Payments
        {
            get => payments;
        }

        public Sales() : base()
        {

        }

        public Sales(Guid customerId, decimal totalPrice, decimal discountPercentage, decimal totalPaid, string remark, decimal taxPercentage = 0) : base(Guid.NewGuid())
        {
            CustomerId = customerId;
            TotalPrice = totalPrice;
            DiscountPercentage = discountPercentage;
            TaxPercentage = taxPercentage;
            TotalPaid = totalPaid;
            Remark = remark;

            // Validate discount
            ValidateDiscount();

            // Set initial status based on payment
            UpdatePaymentStatus();
        }

        public static Sales CreateSalesWithItems(Guid customerId, decimal totalPrice, decimal discountPercentage, decimal totalPaid, string remark, List<SalesItemDetailsDto> salesItemDtos, decimal taxPercentage = 0)
        {
            // Validate total price matches order details
            decimal calculatedTotal = salesItemDtos.Sum(od => od.Quantity * od.UnitPrice);
            if (Math.Abs(calculatedTotal - totalPrice) > 0.01m) // Allow for minor rounding differences
            {
                throw new Common.Exceptions.BusinessLogicException($"Total price mismatch. Provided: {totalPrice:N2}, Calculated from order details: {calculatedTotal:N2}");
            }

            Sales newSales = new(customerId, totalPrice, discountPercentage, totalPaid, remark, taxPercentage);
            newSales.salesItems.AddRange(
                salesItemDtos.Select(salesItem => new SalesItem(newSales.Id,
                salesItem.ProductId,
                salesItem.Quantity,
                salesItem.Unit,
                salesItem.UnitPrice,
                salesItem.UnitCost)).ToList()
            );

            // Raise domain event with order details for stock management
            var salesItemInfos = salesItemDtos.Select(od => new SalesItemInfo(od.ProductId, od.Quantity)).ToList();
            newSales.RaiseDomainEvent(new SalesCreatedEvent(newSales.Id, salesItemInfos));

            return newSales;
        }

        public void Update(Guid customerId, decimal totalPrice, decimal discountPercentage, decimal totalPaid, string remark, List<SalesItemDetailsDto> salesItemDtos, decimal taxPercentage = 0)
        {
            // Validate total price matches order details
            decimal calculatedTotal = salesItemDtos.Sum(od => od.Quantity * od.UnitPrice);
            if (Math.Abs(calculatedTotal - totalPrice) > 0.01m) // Allow for minor rounding differences
            {
                throw new Common.Exceptions.BusinessLogicException($"Total price mismatch. Provided: {totalPrice:N2}, Calculated from order details: {calculatedTotal:N2}");
            }

            // Capture old order details before clearing for stock adjustment
            var oldSalesItemInfos = salesItems.Select(od => new SalesItemInfo(od.ProductId, od.Quantity)).ToList();

            CustomerId = customerId;
            TotalPrice = totalPrice;
            DiscountPercentage = discountPercentage;
            TaxPercentage = taxPercentage;
            TotalPaid = totalPaid;
            Remark = remark;

            // Validate discount
            ValidateDiscount();

            // Clear existing order details and add new ones
            salesItems.Clear();
            salesItems.AddRange(
                salesItemDtos.Select(salesItem => new SalesItem(this.Id,
                salesItem.ProductId,
                salesItem.Quantity,
                salesItem.Unit,
                salesItem.UnitPrice,
                salesItem.UnitCost)).ToList()
            );

            // Raise domain event for stock adjustment
            var newSalesItemInfos = salesItemDtos.Select(od => new SalesItemInfo(od.ProductId, od.Quantity)).ToList();
            RaiseDomainEvent(new SalesUpdatedEvent(this.Id, oldSalesItemInfos, newSalesItemInfos));

            // Update payment status
            UpdatePaymentStatus();
        }

        public void Delete()
        {
            IsDeleted = true;

            // Raise domain event to restore stock for deleted order
            var salesItemInfos = salesItems.Select(od => new SalesItemInfo(od.ProductId, od.Quantity)).ToList();
            RaiseDomainEvent(new SalesDeletedEvent(this.Id, salesItemInfos));
        }

        public void Cancel()
        {
            // Only allow cancellation of pending or partially paid orders
            if (Status == SalesStatus.Paid || Status == SalesStatus.Cancelled)
            {
                throw new Common.Exceptions.BusinessLogicException($"Cannot cancel sales that is {Status}.");
            }

            Status = SalesStatus.Cancelled;

            // Raise domain event to restore stock for cancelled order
            var salesItemInfos = salesItems.Select(od => new SalesItemInfo(od.ProductId, od.Quantity)).ToList();
            RaiseDomainEvent(new SalesCancelledEvent(this.Id, salesItemInfos));
        }

        public void SetSalesNumber(string salesNumber)
        {
            if (string.IsNullOrWhiteSpace(salesNumber))
            {
                throw new Common.Exceptions.BusinessLogicException("Sales number cannot be empty.");
            }
            SalesNumber = salesNumber;
        }

        public static string GenerateSalesNumber(DateTime date, int sequenceNumber)
        {
            var datePrefix = date.ToString("yyyyMMdd");
            return $"SAL-{datePrefix}-{sequenceNumber:D3}";
        }

        public void AddPayment(decimal amount, string paymentMethod, string remark)
        {
            if (amount <= 0)
            {
                throw new Common.Exceptions.BusinessLogicException("Payment amount must be greater than zero.");
            }

            if (Status == SalesStatus.Paid)
            {
                throw new Common.Exceptions.BusinessLogicException("Order is already fully paid.");
            }

            if (Status == SalesStatus.Cancelled)
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

        private void ValidateDiscount()
        {
            // Validate discount percentage
            if (DiscountPercentage < 0 || DiscountPercentage > 100)
            {
                throw new Common.Exceptions.BusinessLogicException("Discount percentage must be between 0 and 100.");
            }

            // Validate that calculated discount amount doesn't exceed total price
            if (DiscountAmount > TotalPrice)
            {
                throw new Common.Exceptions.BusinessLogicException("Calculated discount amount cannot exceed total price.");
            }
        }

        private void UpdatePaymentStatus()
        {
            if (TotalPaid == 0)
            {
                Status = SalesStatus.Pending;
            }
            else if (TotalPaid >= GrandTotal)
            {
                Status = SalesStatus.Paid;
            }
            else
            {
                Status = SalesStatus.PartiallyPaid;
            }
        }
    }
}
