using Application.Features.Product.Commands;
using Application.Features.Tag.Queries;
using Common.Repositories.Interfaces;
using Common.RequestWrapper;
using Domain.Interfaces;

namespace UnitTest
{
    public class ArchitectureTests
    {
        [Fact]
        public void EnsureQueryHandlersUseSpecificDbContext()
        {
            // Define the assembly where query handlers are located.
            var assembly = typeof(GetTagByIdQueryHandler).Assembly;

            // Get type of expected db context
            var readOnlyDbContextType = typeof(IReadOnlyApplicationDbContext);
            var writeDbContextType = typeof(IApplicationDbContext);

            // Get all types in the assembly that implement IQueryHandlerWrapper<,>.
            var queryHandlerTypes = assembly.GetTypes()
                .Where(type => type.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                              i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                .ToList();

            foreach (var queryHandlerType in queryHandlerTypes)
            {
                // Get the constructor parameters of the query handler.
                var constructorParameters = queryHandlerType.GetConstructors()
                    .SelectMany(ctor => ctor.GetParameters())
                    .ToList();

                // Find the parameter of the expected DbContext type.
                var readOnlyDbContextParameter = constructorParameters.FirstOrDefault(parameter =>
                    readOnlyDbContextType.IsAssignableFrom(parameter.ParameterType));
                var writeDbContextParameter = constructorParameters.FirstOrDefault(parameter =>
                    writeDbContextType.IsAssignableFrom(parameter.ParameterType));

                // Assert that the query handler constructor uses the expected DbContext.
                // Should have read only db context.
                Assert.NotNull(readOnlyDbContextParameter);
                // Should not have any write db context.
                Assert.Null(writeDbContextParameter);
            }
        }

        [Fact]
        public void EnsureOnlySingleRepositoryUsedForWritePorposeInCommandHandler()
        {
            // Define the assembly where command handlers are located.
            var assembly = typeof(CreateProductCommand).Assembly;

            // Get all types in the assembly that implement ICommandHandlerWrapper<,>.
            var commandHandlerTypes = assembly.GetTypes()
                .Where(type => type.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                              i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .ToList();

            foreach (var commandHandlerType in commandHandlerTypes)
            {
                // Get the constructor parameters of the command handler.
                var constructorParameters = commandHandlerType.GetConstructors()
                    .SelectMany(ctor => ctor.GetParameters())
                    .ToList();

                // Check if ApplicationDbContext is used
                bool isApplicationDbContextUsed = constructorParameters
                    .Any(param => param.ParameterType == typeof(IApplicationDbContext));

                // Find the number of repository used inside the command handler.
                int numberOfRepository = constructorParameters
                    .Count(param => param.ParameterType
                        .GetInterfaces()
                        .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRepository<>)));

                // Assert that command handler shouldn't use ApplicationDbContext.
                Assert.False(isApplicationDbContextUsed);
                // Assert that number of used repository should be less than 2.
                Assert.True(numberOfRepository < 2);
            }
        }
    }
}
