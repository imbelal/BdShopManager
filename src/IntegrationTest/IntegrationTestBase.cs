using Bogus;
using Domain.Entities;
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
            // Set the environment variable before the tests.
            Environment.SetEnvironmentVariable("TESTING_ENVIRONMENT", "1");
            _client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            CreateFakeTenant(factory.GetTenantId());
        }

        private void CreateFakeTenant(Guid tenantId)
        {
            var faker = new Faker<Tenant>()
                    .RuleFor(u => u.Id, f => tenantId)
                    .RuleFor(u => u.Name, f => f.Person.FullName)
                    .RuleFor(u => u.Address, f => f.Person.Address.Street)
                    .RuleFor(u => u.PhoneNumber, f => f.Person.Phone);

            Tenant fakeTenant = faker.Generate();

            _context.Tenants.Add(fakeTenant);
            _context.SaveChangesAsync(default).GetAwaiter().GetResult();
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
