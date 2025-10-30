using Common.Entities;

namespace Domain.Entities
{
    public class Payment : AuditableTenantEntityBase<Guid>
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, BankTransfer, etc.
        public string Remark { get; set; } = string.Empty;

        public virtual Order Order { get; set; }
        public Tenant Tenant { get; set; }

        public Payment() : base()
        {

        }

        public Payment(Guid orderId, decimal amount, string paymentMethod, string remark) : base(Guid.NewGuid())
        {
            OrderId = orderId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Remark = remark;
        }
    }
}
