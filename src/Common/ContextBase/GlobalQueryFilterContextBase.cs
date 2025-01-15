using Common.Entities.Interfaces;
using Common.Extensions;
using Common.Interceptor;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Common.ContextBase
{
    public class GlobalQueryFilterContextBase<TContext> : DbContext where TContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private Guid? tenantId = null;

        public GlobalQueryFilterContextBase(DbContextOptions options, ICurrentUserService currentUserService) :
            base(options)
        {
            _currentUserService = currentUserService;
            string id = _currentUserService?.GetUser()?.Claims?.FirstOrDefault(c => c.Type == "tenantId")?.Value;
            if (!id.IsNullOrEmpty())
            {
                tenantId = Guid.Parse(id);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyQueryFilter<ISoftDeletable>(e => e.IsDeleted == false);
            modelBuilder.ApplyQueryFilter<IAuditableTenantEntity>(e => tenantId == null || e.TenantId == tenantId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
