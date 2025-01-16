using Common.Entities.Interfaces;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Common.ContextBase
{
    public class AuditContextBase<TContext> : GlobalQueryFilterContextBase<TContext> where TContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IHostEnvironment _hostingEnvironment;
        public AuditContextBase(string connectionString, IDbContextOptionsProvider dbContextOptionsProvider, ICurrentUserService currentUserService, IHostEnvironment hostingEnvironment)
            : base(dbContextOptionsProvider.CreateDbContextOptions(connectionString), currentUserService)
        {
            _currentUserService = currentUserService;
            _hostingEnvironment = hostingEnvironment;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditProperties();
            SetTenantId();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditProperties()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditableEntity entity)
                {
                    var now = DateTime.UtcNow;
                    string user = _currentUserService?.GetUser()?.Claims?.FirstOrDefault(x => x.Type == "username")?.Value ?? "System";

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entity.CreatedBy = user;
                            entity.CreatedUtcDate = now;
                            entity.UpdatedBy = String.Empty;
                            break;
                        case EntityState.Modified:
                            entity.UpdatedBy = user;
                            entity.UpdatedUtcDate = now;
                            break;
                    }
                }
            }
        }

        private void SetTenantId()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                // Check if the entity has a TenantId property
                if (entry.Entity.GetType().GetProperty("TenantId") != null)
                {
                    // Only set TenantId for newly added entities
                    if (entry.State == EntityState.Added)
                    {
                        var tenantIdProperty = entry.Entity.GetType().GetProperty("TenantId");

                        // Ensure the TenantId property is writable
                        if (tenantIdProperty != null && tenantIdProperty.CanWrite)
                        {
                            // Get the current value of TenantId
                            var currentTenantId = tenantIdProperty.GetValue(entry.Entity);

                            // Set the TenantId only if it's not already set (null or default)
                            if (currentTenantId == null || Guid.TryParse(currentTenantId.ToString(), out var guid) && guid == Guid.Empty)
                            {
                                // get tenant id
                                Guid tenantId = Guid.Parse(_currentUserService?.GetUser()?.Claims?.FirstOrDefault(c => c.Type == "tenantId")?.Value);
                                tenantIdProperty.SetValue(entry.Entity, tenantId);
                            }
                        }
                    }
                }
            }
        }

    }
}
