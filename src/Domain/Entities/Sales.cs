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
        public decimal TaxPercentage { get; set; } = 0;
        public decimal TotalPaid { get; private set; }
        public SalesStatus Status { get; private set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        // Calculated properties
        public decimal TaxAmount => TotalPrice * (TaxPercentage / 100);
        public decimal GrandTotal => TotalPrice + TaxAmount;

        // Profit calculation properties
        public decimal SalesTotalCost => salesItems.Sum(od => od.TotalCost);
        public decimal SalesProfit => TotalPrice - SalesTotalCost;
        public decimal SalesProfitMargin => TotalPrice > 0
            ? Math.Round((SalesProfit / TotalPrice) * 100, 2)
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

        public Sales(Guid customerId, decimal totalPrice, decimal totalPaid, string remark, decimal taxPercentage = 0) : base(Guid.NewGuid())
        {
            CustomerId = customerId;
            TotalPrice = totalPrice;
            TaxPercentage = taxPercentage;
            TotalPaid = totalPaid;
            Remark = remark;

            // Set initial status based on payment
            UpdatePaymentStatus();
        }

        public static Sales CreateSalesWithItems(Guid customerId, decimal totalPrice, decimal totalPaid, string remark, List<SalesItemDetailsDto> salesItemDtos, decimal taxPercentage = 0)
        {
            // Validate total price matches order details
            decimal calculatedTotal = salesItemDtos.Sum(od => od.Quantity * od.UnitPrice);
            if (Math.Abs(calculatedTotal - totalPrice) > 0.01m) // Allow for minor rounding differences
            {
                throw new Common.Exceptions.BusinessLogicException($"Total price mismatch. Provided: {totalPrice:N2}, Calculated from order details: {calculatedTotal:N2}");
            }

            Sales newSales = new(customerId, totalPrice, totalPaid, remark, taxPercentage);
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

        public void Update(Guid customerId, decimal totalPrice, decimal totalPaid, string remark, List<SalesItemDetailsDto> salesItemDtos, decimal taxPercentage = 0)
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
            TaxPercentage = taxPercentage;
            TotalPaid = totalPaid;
            Remark = remark;

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
