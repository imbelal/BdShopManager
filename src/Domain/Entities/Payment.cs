using Common.Entities;

namespace Domain.Entities
{
    public class Payment : AuditableTenantEntityBase<Guid>
    {
        public Guid SalesId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, BankTransfer, etc.
        public string Remark { get; set; } = string.Empty;

        public virtual Sales Sales { get; set; }
        public Tenant Tenant { get; set; }

        public Payment() : base()
        {

        }

        public Payment(Guid salesId, decimal amount, string paymentMethod, string remark) : base(Guid.NewGuid())
        {
            SalesId = salesId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Remark = remark;
        }
    }
}
