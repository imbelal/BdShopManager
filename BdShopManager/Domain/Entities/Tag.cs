using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class Tag : AuditableEntityBase, IAggregateRoot
    {
        public string Title { get; set; }
    }
}
