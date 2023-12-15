using Microsoft.EntityFrameworkCore;

namespace Common.ContextBase
{
    public interface IDbContextOptionsProvider
    {
        DbContextOptions<DbContext> CreateDbContextOptions(string connectionString);
    }
}
