using Common.Entities.Interfaces;

namespace Common.Entities
{
    public class AuditableTenantEntityBase<TIdentity> : AuditableEntityBase<TIdentity>, IAuditableTenantEntity
    {
        public Guid TenantId { get; set; }

        public AuditableTenantEntityBase()
        {

        }

        public AuditableTenantEntityBase(TIdentity id) : base(id) { }
    }
}
