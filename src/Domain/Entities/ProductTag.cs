using Common.Entities;

namespace Domain.Entities
{
    public class ProductTag : AuditableTenantEntityBase<Guid>
    {
        public Guid ProductId { get; set; }
        public Guid TagId { get; set; }
        public virtual Product Product { get; set; }
        public Tenant Tenant { get; set; }

        public ProductTag() : base()
        {

        }

        public ProductTag(Guid productId, Guid tagId) : base(Guid.NewGuid())
        {
            ProductId = productId;
            TagId = tagId;
        }
    }
}
