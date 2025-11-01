using Common.Services.Interfaces;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Domain.EventHandlers
{
    public class InvalidateGetAllCategoryQueryCacheEventHandler : INotificationHandler<CategoryCreatedEvent>, INotificationHandler<CategoryUpdatedEvent>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly string _cacheKey;

        public InvalidateGetAllCategoryQueryCacheEventHandler(IMemoryCache memoryCache, ICurrentUserService currentUserService)
        {
            _memoryCache = memoryCache;
            string tenantId = currentUserService?.GetUser()?.Claims?.FirstOrDefault(c => c.Type == "tenantId")?.Value;
            _cacheKey = $"allCategory_{tenantId}";
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

        private void InvalidateAllCategoryQuery()
        {
            _memoryCache.Remove(_cacheKey);
        }
    }
}
