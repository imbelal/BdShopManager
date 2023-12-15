using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTest
{
    public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        protected readonly HttpClient _client;
        protected readonly IApplicationDbContext _context;
        protected readonly IMediator _mediator;
        public IntegrationTestBase(CustomWebApplicationFactory<Program> factory)
        {
            // Set the environment variable before the tests
            Environment.SetEnvironmentVariable("TESTING_ENVIRONMENT", "1");
            _client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        }

        public void Dispose()
        {
            _client.Dispose();
            _context.Dispose();
            // Reset the environment variable after the tests
            Environment.SetEnvironmentVariable("TESTING_ENVIRONMENT", null);

        }
    }
}
