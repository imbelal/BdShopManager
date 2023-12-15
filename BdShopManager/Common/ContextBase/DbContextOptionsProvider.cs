using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Common.ContextBase
{
    public class DbContextOptionsProvider : IDbContextOptionsProvider
    {
        public DbContextOptions<DbContext> CreateDbContextOptions(string connectionString)
        {
            DbContextOptionsBuilder<DbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<DbContext>();
            dbContextOptionsBuilder = dbContextOptionsBuilder.UseSqlServer(connectionString, delegate (SqlServerDbContextOptionsBuilder x)
            {
                x.CommandTimeout(320).UseNetTopologySuite();
            });
            return dbContextOptionsBuilder.Options;
        }
    }
}
