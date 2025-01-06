namespace Common.Entities.Interfaces
{
    public interface IAuditableTenantEntity : IAuditableEntity
    {
        Guid TenantId { get; set; }
    }
}
