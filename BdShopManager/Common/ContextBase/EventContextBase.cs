using Common.Entities;
using Common.Events;
using Common.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.ContextBase
{
    public class EventContextBase<TContext> : AuditContextBase<TContext> where TContext : DbContext
    {
        private readonly IPublisher _publisher;
        private readonly ILogger<EventContextBase<TContext>> _logger;
        public EventContextBase(string connectionString, IDbContextOptionsProvider dbContextOptionsProvider, ICurrentUserService currentUserService, IPublisher publisher, IHostEnvironment hostingEnvironment, ILogger<EventContextBase<TContext>> logger)
            : base(connectionString, dbContextOptionsProvider, currentUserService, hostingEnvironment)
        {
            _publisher = publisher;
            _logger = logger;
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            using (_logger.BeginScope("UnitOfWork.SaveChangesAsync"))
            {
                int result;
                using (IDbContextTransaction transaction = Database.BeginTransaction())
                {
                    using (_logger.BeginScope("DbContextTransaction"))
                    {
                        try
                        {
                            List<IDomainEvent> domainEvents = GetDomainEvents();
                            result = await base.SaveChangesAsync(cancellationToken);
                            _logger.LogDebug("DbContext.SaveChangesAsync");
                            await PublishDomainEventsAsync(domainEvents, cancellationToken);
                            transaction.Commit(); // Commit the transaction
                            _logger.LogDebug("Transaction committed");
                        }
                        catch
                        {

                            transaction.Rollback();
                            _logger.LogDebug("Transaction rolled back");
                            throw;
                        }
                    }
                }
                return result;
            }
        }

        private List<IDomainEvent> GetDomainEvents()
        {
            var entries = ChangeTracker.Entries();

            List<IDomainEvent> domainEvents = new();

            foreach (var entry in entries)
            {
                if (entry.Entity is EntityBase entity)
                {
                    domainEvents.AddRange(entity.GetDomainEvents());
                    entity.ClearDomainEvents();
                }
            }
            return domainEvents;
        }

        private async Task PublishDomainEventsAsync(List<IDomainEvent> domainEvents, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!domainEvents.Any())
            {
                return;
            }

            using (_logger.BeginScope("Dispatch domain events before commit"))
            {
                foreach (IDomainEvent domainEvent in domainEvents)
                {
                    using (_logger.BeginScope("Dispatch domain event [" + domainEvent.GetType().Name + "]"))
                    {
                        await _publisher.Publish(domainEvent, cancellationToken);
                        List<IDomainEvent> nestedDomainEvents = GetDomainEvents();
                        await base.SaveChangesAsync(cancellationToken);
                        _logger.LogDebug("DbContext.SaveChangesAsync");
                        await PublishDomainEventsAsync(nestedDomainEvents, cancellationToken);
                    }
                }
            }
        }
    }
}
