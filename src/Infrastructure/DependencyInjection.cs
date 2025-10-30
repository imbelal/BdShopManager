using Common.ContextBase;
using Common.Repositories.Interfaces;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
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

            foreach (var type in list)
            {
                var interfaceType = type.GetInterfaces().FirstOrDefault(x => x.Name != typeof(IRepository<>).Name);
                if (interfaceType != null)
                {
                    services.AddTransient(interfaceType, type);
                }
            }

            // Register Azure Storage Service
            services.AddTransient<IFileStorageService, AzureBlobStorageService>();

            // Register PDF Generator Service
            services.AddTransient<IPdfGeneratorService, PdfGeneratorService>();
        }
    }
}
