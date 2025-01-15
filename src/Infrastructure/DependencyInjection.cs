using Common.ContextBase;
using Common.Repositories.Interfaces;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Write db context.
            services.AddSingleton<IDbContextOptionsProvider, DbContextOptionsProvider>();
            services.AddSingleton<IDbConnectionStringProvider>(new DbConnectionStringProvider(
               configuration.GetConnectionString("DefaultConnection"),
               configuration.GetConnectionString("ReadOnlyConnection") ?? configuration.GetConnectionString("DefaultConnection")
           ));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddTransient<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);
            services.AddTransient<IReadOnlyApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);

            services.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));
            // Find all other repository types except generic reository.
            var list = Assembly.GetExecutingAssembly().GetTypes()
                .Where(mytype => mytype.GetInterface(typeof(IRepository<>).Name) != null && !mytype.IsInterface && mytype != typeof(GenericRepository<>))
                .ToList();

            foreach (var item in list)
            {
                string interfaceName = $"I{item.Name}";
                Type? usedInteface = item.GetInterface(interfaceName);
                if (usedInteface is not null)
                {
                    services.AddTransient(usedInteface, item);
                }
            }
        }
    }
}
