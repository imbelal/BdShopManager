using Common.Entities;

namespace Domain.Entities
{
    public class ProductTag : AuditableEntityBase
    {
        public Guid ProductId { get; set; }
        public Guid TagId { get; set; }
        public virtual Product Product { get; set; }

        public ProductTag()
        {

        }

        public ProductTag(Guid productId, Guid tagId)
        {
            ProductId = productId;
            TagId = tagId;
        }
    }
}
