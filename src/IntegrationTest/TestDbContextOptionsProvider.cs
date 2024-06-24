using Common.ContextBase;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest
{
    public class TestDbContextOptionsProvider : IDbContextOptionsProvider
    {
        public DbContextOptions<DbContext> CreateDbContextOptions(string connectionString)
        {
            DbContextOptionsBuilder<DbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<DbContext>();
            dbContextOptionsBuilder = dbContextOptionsBuilder.UseSqlite(connectionString, x => x.UseNetTopologySuite());
            return dbContextOptionsBuilder.Options;
        }
    }
}
