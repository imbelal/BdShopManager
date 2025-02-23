using Common.ContextBase;
using Common.Services.Interfaces;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IntegrationTest.Helpers
{
    public class TestDbContext : ApplicationDbContext
    {
        public TestDbContext(IDbConnectionStringProvider dbConnectionStringProvider, IDbContextOptionsProvider dbContextOptionsProvider, ICurrentUserService currentUserService, IPublisher publisher, IHostEnvironment hostingEnvironment, ILogger<TestDbContext> logger)
        : base(dbConnectionStringProvider, dbContextOptionsProvider, currentUserService, publisher, hostingEnvironment, logger)
        {
            var dbState = Database.GetDbConnection().State;
            if (dbState == System.Data.ConnectionState.Closed)
            {
                Database.OpenConnection();
            }
        }
    }
}
