using Common.Entities.Interfaces;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Common.ContextBase
{
    public class AuditContextBase<TContext> : SoftDeletableContextBase<TContext> where TContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IHostEnvironment _hostingEnvironment;
        public AuditContextBase(string connectionString, IDbContextOptionsProvider dbContextOptionsProvider, ICurrentUserService currentUserService, IHostEnvironment hostingEnvironment)
            : base(dbContextOptionsProvider.CreateDbContextOptions(connectionString))
        {
            _currentUserService = currentUserService;
            _hostingEnvironment = hostingEnvironment;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditProperties();
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
                    string user;
                    if (_hostingEnvironment.IsDevelopment())
                    {
                        user = "System from dev env.";
                    }
                    else
                    {
                        user = _currentUserService.GetUser().Identity?.Name ?? "System";
                    }

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
    }
}
