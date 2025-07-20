using Common.ContextBase;
using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace IntegrationTest.Helpers
{
    public class CustomWebApplicationFactory<TProgram>(string connectionString) : WebApplicationFactory<TProgram>
        where TProgram : class
    {
        private readonly Guid _tenantId = Guid.NewGuid();
        private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        public Mock<IFileStorageService> MockFileStorageService { get; } = new();
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
                services.AddSingleton<IDbConnectionStringProvider>(new TestDbConnectionProvider(_connectionString));

                // Use TestDbContext instead of ApplicationDbContext
                services.AddDbContext<TestDbContext>();
                services.AddScoped<IApplicationDbContext>(provider => provider.GetService<TestDbContext>()!);
                services.AddScoped<IReadOnlyApplicationDbContext>(provider => provider.GetService<TestDbContext>()!);

                // Register MediatR
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

                // Register controller as service
                services.AddControllers()
                    .AddApplicationPart(typeof(Program).Assembly)
                    .AddControllersAsServices();

                // Register a mocked IHttpContextAccessor
                CreateMockHttpContextAccessor(services);

                // Remove existing IFileStorageService registration if any
                var fileStorageDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFileStorageService));
                if (fileStorageDescriptor != null)
                {
                    services.Remove(fileStorageDescriptor);
                }

                // Add a mock IFileStorageService for testing
                MockFileStorageService
                    .Setup(x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync("https://test-storage.blob.core.windows.net/product-photos/test-image.jpg");
                MockFileStorageService
                    .Setup(x => x.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);
                MockFileStorageService
                    .Setup(x => x.GetFileUrl(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns("https://test-storage.blob.core.windows.net/product-photos/test-image.jpg");

                services.AddSingleton(MockFileStorageService.Object);
            });

            builder.UseEnvironment("Development");
        }

        private void RemoveAllDbContextsFromServices(IServiceCollection services)
        {
            var descriptors = services.Where(d => d.ServiceType.BaseType == typeof(DbContextOptions)).ToList();
            descriptors.ForEach(d => services.Remove(d));

            var dbConnectionDescriptor = services.Where(d => d.ServiceType == typeof(IDbConnectionStringProvider)).ToList();
            dbConnectionDescriptor.ForEach(d => services.Remove(d));

            var dbContextDescriptor = services.Where(d => d.ServiceType == typeof(IDbContextOptionsProvider)).ToList();
            dbContextDescriptor.ForEach(d => services.Remove(d));
        }

        private void CreateMockHttpContextAccessor(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHttpContextAccessor));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

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
            mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);

            services.AddSingleton(mockHttpContextAccessor.Object);
        }
    }
}