using Common.ContextBase;
using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest
{
    public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove all existing contexts
                RemoveAllDbContextsFromServices(services);

                // Register TestDbContextOptionsProvider as IDbContextOptionsProvider
                services.AddSingleton<IDbContextOptionsProvider, TestDbContextOptionsProvider>();
                services.AddSingleton<IDbConnectionStringProvider>(new TestDbConnectionProvider("DataSource=:memory:"));


                // Use TestDbContext instead of ApplicationDbContext.
                services.AddScoped<IApplicationDbContext>(provider => provider.GetService<TestDbContext>()!);
                services.AddScoped<IReadOnlyApplicationDbContext>(provider => provider.GetService<TestDbContext>()!);
                services.AddDbContext<TestDbContext>();

            });

            builder.UseEnvironment("Development");
        }

        private void RemoveAllDbContextsFromServices(IServiceCollection services)
        {
            // reverse operation of AddDbContext<XDbContext> which removes  DbContexts from services
            var descriptors = services.Where(d => d.ServiceType.BaseType == typeof(DbContextOptions)).ToList();
            descriptors.ForEach(d => services.Remove(d));

            // Remove exisitng connections
            var dbConnectionDescriptor = services.Where(
                d => d.ServiceType == typeof(IDbConnectionStringProvider)).ToList();
            dbConnectionDescriptor.ForEach(d => services.Remove(d));

            // Remove exisitng context options providers
            var dbContextDescriptor = services.Where(
                d => d.ServiceType == typeof(IDbContextOptionsProvider)).ToList();
            dbContextDescriptor.ForEach(d => services.Remove(d));
        }
    }
}
