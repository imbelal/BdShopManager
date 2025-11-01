using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Dtos;
using Domain.Events;

namespace Domain.Entities
{
    public class SalesReturn : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        private List<SalesReturnItem> salesReturnItems = new();
        public string ReturnNumber { get; private set; } = string.Empty;
        public Guid SalesId { get; set; }
        public decimal TotalRefundAmount { get; set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        public IReadOnlyCollection<SalesReturnItem> SalesReturnItems
        {
            get => salesReturnItems;
        }

        public SalesReturn() : base()
        {

        }

        public SalesReturn(Guid salesId, decimal totalRefundAmount, string remark) : base(Guid.NewGuid())
        {
            SalesId = salesId;
            TotalRefundAmount = totalRefundAmount;
            Remark = remark;
        }

        public static SalesReturn CreateSalesReturnWithItems(Guid salesId, decimal totalRefundAmount, string remark, List<SalesReturnItemDetailsDto> salesReturnItemDtos)
        {
            // Validate total refund matches return items
            decimal calculatedTotal = salesReturnItemDtos.Sum(item => item.Quantity * item.UnitPrice);
            if (Math.Abs(calculatedTotal - totalRefundAmount) > 0.01m) // Allow for minor rounding differences
            {
                throw new Common.Exceptions.BusinessLogicException($"Total refund amount mismatch. Provided: {totalRefundAmount:N2}, Calculated from return items: {calculatedTotal:N2}");
            }

            SalesReturn newSalesReturn = new(salesId, totalRefundAmount, remark);
            newSalesReturn.salesReturnItems.AddRange(
                salesReturnItemDtos.Select(item => new SalesReturnItem(
                    newSalesReturn.Id,
                    item.ProductId,
                    item.Quantity,
                    item.Unit,
                    item.UnitPrice,
                    item.Reason)).ToList()
            );

            // Raise domain event with return item details for stock management
            var returnItemInfos = salesReturnItemDtos.Select(item =>
                new SalesReturnItemInfo(item.ProductId, item.Quantity)).ToList();
            newSalesReturn.RaiseDomainEvent(new SalesReturnCreatedEvent(newSalesReturn.Id, returnItemInfos));

            return newSalesReturn;
        }

        public void Delete()
        {
            IsDeleted = true;

            // Raise domain event to decrease stock for deleted return (reverse the stock increase)
            var returnItemInfos = salesReturnItems.Select(item =>
                new SalesReturnItemInfo(item.ProductId, item.Quantity)).ToList();
            RaiseDomainEvent(new SalesReturnDeletedEvent(this.Id, returnItemInfos));
        }

        public void SetReturnNumber(string returnNumber)
        {
            if (string.IsNullOrWhiteSpace(returnNumber))
            {
                throw new Common.Exceptions.BusinessLogicException("Return number cannot be empty.");
            }
            ReturnNumber = returnNumber;
        }

        public static string GenerateReturnNumber(DateTime date, int sequenceNumber)
        {
            var datePrefix = date.ToString("yyyyMMdd");
            return $"RET-{datePrefix}-{sequenceNumber:D3}";
        }
    }
}
