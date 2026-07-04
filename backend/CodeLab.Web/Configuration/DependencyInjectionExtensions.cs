using CodeLab.Core;
using CodeLab.Core.Abstractions.Endpoints;
using CodeLab.Infrastructure.FileSystem;
using CodeLab.Infrastructure.Postgres;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;

namespace CodeLab.Web.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSerilogLogging(configuration)
            .AddOpenApiSpecification()
            .AddEndpoints(typeof(IEndpoint).Assembly);

        services
            .AddCore(configuration)
            .AddInfrastructurePostgres(configuration)
            .AddInfrastructureFileSystem(configuration);

        return services;
    }

    private static IServiceCollection AddOpenApiSpecification(this IServiceCollection services)
    {
        services.AddOpenApi();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Code Lab API", Version = "v1" });
        });

        return services;
    }

    private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((sp, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(sp)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("ServiceName", "CodeLab"));

        return services;
    }
}