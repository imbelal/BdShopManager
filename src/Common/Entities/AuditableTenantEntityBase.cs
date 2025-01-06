using Common.Entities.Interfaces;

namespace Common.Entities
{
    public class AuditableTenantEntityBase : AuditableEntityBase, IAuditableTenantEntity
    {
        public Guid TenantId { get; set; }
    }
}
