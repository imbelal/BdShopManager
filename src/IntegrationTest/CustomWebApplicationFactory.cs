using Common.ContextBase;
using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace IntegrationTest
{
    public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly Guid _tenantId = Guid.NewGuid();

        public Guid GetTenantId()
        {
            return _tenantId;
        }

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

                // Register a mocked IHttpContextAccessor
                CreateMockHttpContextAccessor(services);
            });

            builder.UseEnvironment("Development");
        }

        private void RemoveAllDbContextsFromServices(IServiceCollection services)
        {
            // reverse operation of AddDbContext<XDbContext> which removes  DbContexts from services
            var descriptors = services.Where(d => d.ServiceType.BaseType == typeof(DbContextOptions)).ToList();
            descriptors.ForEach(d => services.Remove(d));

            // Remove existing connections
            var dbConnectionDescriptor = services.Where(
                d => d.ServiceType == typeof(IDbConnectionStringProvider)).ToList();
            dbConnectionDescriptor.ForEach(d => services.Remove(d));

            // Remove existing context options providers
            var dbContextDescriptor = services.Where(
                d => d.ServiceType == typeof(IDbContextOptionsProvider)).ToList();
            dbContextDescriptor.ForEach(d => services.Remove(d));
        }

        private void CreateMockHttpContextAccessor(IServiceCollection services)
        {
            // Remove the default IHttpContextAccessor registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHttpContextAccessor));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Set up the mock HttpContext
            var claims = new List<Claim>
            {
                new Claim("username", "TestUser"),
                new Claim("tenantId", _tenantId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.User).Returns(claimsPrincipal);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            // Configure the accessor to return the mocked HttpContext
            mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);

            services.AddSingleton(mockHttpContextAccessor.Object);
        }
    }
}
