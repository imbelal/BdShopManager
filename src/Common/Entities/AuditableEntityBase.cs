using Common.Entities.Interfaces;

namespace Common.Entities
{
    public abstract class AuditableEntityBase : EntityBase, IAuditableEntity
    {
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedUtcDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedUtcDate { get; set; }
    }
}
