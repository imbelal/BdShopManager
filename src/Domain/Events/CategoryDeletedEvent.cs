using Common.Events;

namespace Domain.Events
{
    public class CategoryDeletedEvent : IDomainEvent
    {
        public Guid CategoryId { get; set; }

        public CategoryDeletedEvent(Guid categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
