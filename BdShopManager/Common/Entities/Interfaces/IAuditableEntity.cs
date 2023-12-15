namespace Common.Entities.Interfaces
{
    public interface IAuditableEntity
    {
        string CreatedBy { get; set; }
        DateTimeOffset CreatedUtcDate { get; set; }
        string? UpdatedBy { get; set; }
        DateTimeOffset? UpdatedUtcDate { get; set; }
    }
}
