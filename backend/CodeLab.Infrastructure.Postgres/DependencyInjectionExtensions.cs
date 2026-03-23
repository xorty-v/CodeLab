using CodeLab.Core;
using CodeLab.Core.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeLab.Infrastructure.Postgres;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructurePostgres(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Scan(scan => scan
            .FromAssemblies(typeof(DependencyInjectionExtensions).Assembly)
            .AddClasses(classes => classes
                .Where(type => type.Name.EndsWith("Repository")), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddDbContextPool<ApplicationDbContext>((provider, options) =>
        {
            string? connectionString = configuration.GetConnectionString(ConnectionStringNames.DATABASE);
            IHostEnvironment environment = provider.GetRequiredService<IHostEnvironment>();
            ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();

            options.UseNpgsql(connectionString);

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            options.UseLoggerFactory(loggerFactory);
            options.UseSnakeCaseNamingConvention();
        });

        services.AddDbContextPool<IApplicationReadDbContext, ApplicationDbContext>((provider, options) =>
        {
            string? connectionString = configuration.GetConnectionString(ConnectionStringNames.DATABASE);
            IHostEnvironment hostEnvironment = provider.GetRequiredService<IHostEnvironment>();
            ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();

            options.UseNpgsql(connectionString);

            if (hostEnvironment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            options.UseLoggerFactory(loggerFactory);
        });

        return services;
    }
}