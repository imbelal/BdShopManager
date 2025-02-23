using Bogus;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTest.Helpers
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected HttpClient Client { get; }
        protected IServiceScope Scope { get; }
        protected IApplicationDbContext Context { get; }
        protected IMediator Mediator { get; private set; }
        protected CustomWebApplicationFactory<Program> Factory { get; }

        protected IntegrationTestBase()
        {
            Factory = new CustomWebApplicationFactory<Program>(TestDatabaseFixture.ConnectionString);
            Client = Factory.CreateClient();

            Scope = Factory.Services.CreateScope();
            Context = Scope.ServiceProvider.GetRequiredService<TestDbContext>();
            Mediator = Scope.ServiceProvider.GetRequiredService<IMediator>();
        }

        private void CreateFakeTenant(Guid tenantId)
        {
            var faker = new Faker<Tenant>()
                .RuleFor(u => u.Id, f => tenantId)
                .RuleFor(u => u.Name, f => f.Person.FullName)
                .RuleFor(u => u.Address, f => f.Person.Address.Street)
                .RuleFor(u => u.PhoneNumber, f => f.Person.Phone[..10]);

            Tenant fakeTenant = faker.Generate();
            Context.Tenants.Add(fakeTenant);
            Context.SaveChangesAsync(default).GetAwaiter().GetResult();
        }

        public Task InitializeAsync()
        {
            CreateFakeTenant(Factory.GetTenantId());
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            Client?.Dispose();
            Context?.Dispose();
            Factory?.Dispose();
            return Task.CompletedTask;
        }

        protected TController ResolveController<TController>()
            where TController : ControllerBase
        {
            return Scope.ServiceProvider.GetRequiredService<TController>();
        }

        /// <summary>
        /// Executes a controller method and returns the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the controller method.</typeparam>
        /// <param name="controllerMethod">The controller method to execute (e.g., myController.Create(param)).</param>
        /// <param name="preparedEntities">Seeding objects.</param>
        /// <returns>The result of the controller method.</returns>
        protected async Task<TResult> ExecuteControllerMethodAsync<TResult>(
            Func<Task<TResult>> controllerMethod,
            List<Object>? preparedEntities = null)
        {
            if (preparedEntities != null)
            {
                AddEntities(preparedEntities).GetAwaiter().GetResult();
            }

            // Execute the controller method and return the result
            return await controllerMethod();
        }

        /// <summary>
        /// Add seeding entities.
        /// </summary>
        /// <param name="preparedEntities"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task AddEntities(List<Object> preparedEntities)
        {
            // Add and save prepared entities
            foreach (var entity in preparedEntities)
            {
                // Get the entity type
                Type entityType = entity.GetType();

                // Get the DbSet for this entity type using reflection
                var method = typeof(IApplicationDbContext)
                    .GetMethod("GetDbSet")
                    ?.MakeGenericMethod(entityType);

                if (method == null)
                {
                    throw new InvalidOperationException($"Could not find DbSet for type {entityType.Name}");
                }

                var dbSet = method.Invoke(Context, null);

                // Use reflection to call DbSet.Add
                var addMethod = dbSet?.GetType().GetMethod("Add");
                addMethod?.Invoke(dbSet, new[] { entity });
            }

            // Save changes before executing the controller method
            await Context.SaveChangesAsync(default);
        }
    }
}