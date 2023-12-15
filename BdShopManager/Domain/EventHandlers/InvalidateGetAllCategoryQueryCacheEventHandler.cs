using Domain.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Domain.EventHandlers
{
    public class InvalidateGetAllCategoryQueryCacheEventHandler : INotificationHandler<CategoryCreatedEvent>, INotificationHandler<CategoryUpdatedEvent>, INotificationHandler<CategoryDeletedEvent>
    {
        private readonly IMemoryCache _memoryCache;
        public InvalidateGetAllCategoryQueryCacheEventHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public Task Handle(CategoryCreatedEvent notification, CancellationToken cancellationToken)
        {
            InvalidateAllCategoryQuery();

            return Task.CompletedTask;
        }
        public Task Handle(CategoryUpdatedEvent notification, CancellationToken cancellationToken)
        {
            InvalidateAllCategoryQuery();

            return Task.CompletedTask;
        }
        public Task Handle(CategoryDeletedEvent notification, CancellationToken cancellationToken)
        {
            InvalidateAllCategoryQuery();

            return Task.CompletedTask;
        }

        private void InvalidateAllCategoryQuery()
        {
            _memoryCache.Remove("allCategory");
        }
    }
}
