using Common.Entities;

namespace Domain.Entities
{
    public class ProductTag : AuditableEntityBase
    {
        public Guid PostId { get; set; }
        public Guid TagId { get; set; }
        public virtual Product Post { get; set; }

        public ProductTag()
        {

        }

        public ProductTag(Guid postId, Guid tagId)
        {
            PostId = postId;
            TagId = tagId;
        }
    }
}
