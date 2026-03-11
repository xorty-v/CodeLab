using CodeLab.Core;
using CodeLab.Core.Endpoints;
using CodeLab.Infrastructure.Postgres;
using CodeLab.SubmissionProcessing;
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
            .AddSubmissionProcessing(configuration)
            .AddInfrastructurePostgres(configuration);

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